# RSM integration

How RSM US LLP users sign in to OPL, how access is limited, and how to tell RSM staff
from their Alliance firm users. Written September 2026 during AIC-87, so this research
doesn't have to be redone.

## Background

RSM US LLP was named **McGladrey** until 2015, so every identifier in the code and
database uses McGladrey names:

| Identifier | Meaning |
|---|---|
| `Mcgdy` | Referring site used by **all** RSM traffic: RSM staff *and* Alliance firm users |
| `Mcgdyasc` | "McGladrey Associates" referring site. Configured but **unused**: only 7 old Knowlysis/McGladrey test accounts |
| `mcgladrey` | Content domain string RSM's portals send in `hidDomain` (the subscriptions a user gets) |

RSM has a licensing agreement with AICPA (owned on the AICPA side by Kyle George,
Licensing & Rights, who took over from Mary Walter) for direct login access. RSM also
provides OPL access to people at firms in its **RSM Alliance** and **AdvanceCPA**
programs, through the client portal of its Professional Services+ (PS+) offering.

## How a login works

RSM has two portals that link to OPL:

1. **Employee portal:** used by RSM staff.
2. **PS+ client portal:** used by Alliance/AdvanceCPA firm users.

Both portals do the same two-step handshake, server to server:

```
RSM portal
  1. SOAP call: GetAuthorizationInformationForUser(email, "Mcgdy")
       -> https://raws.aicpa.org/Authentication.svc   (SingleSignOnHelper project)
       <- returns a session token (GUID), or "Error: ..." on failure
  2. Browser POST (form fields) to https://library.aicpa.org/mainui/SeamlessLogin.aspx
       hidSecurityToken = token from step 1
       hidUserId        = user's email
       hidReferringSite = Mcgdy
       hidDomain        = mcgladrey
       hidRelayState    = optional page to land on
  3. OPL looks up the D_User row by email + token, logs the user on with the
     content domain RSM sent, checks the concurrency limit, and redirects
     to hidRelayState or /default.
```

If a user reaches `SeamlessLogin.aspx` without the POST (for example by clicking a plain
link), they get "you don't have access." That page only works when it receives the POST.

### Code map

| Step | File | What it does |
|---|---|---|
| Token service | `_code/SingleSignOnHelper/Authentication.svc.cs` | `GetAuthorizationInformationForUser` returns the user's session ID, creating one if needed |
| User auto-creation | `_code/D_DestroyerAPI/User/UserDalc.cs` `GetSessionByEmail` | **Creates a D_User row for any email it hasn't seen** under that referring site |
| Login page | `_code/MainUI/MainUI/SeamlessLogin.aspx.cs` | `MCGDY` / `MCGDYASC` branch: builds the user from email + token and calls `LogOn(sessionId, hidDomain)`. No allow-list check |
| Concurrency check | `_code/D_DestroyerAPI/User/UserSecurity.cs` `SetMcgdySubscriptionContext` and the `Mcgdy` branch of `AuthenticateUser` | Reads `<ReferringSite>MaxUsers` from Web.config and refuses the login if the firm's active-session count has reached it (unless the user already has a session) |
| Session storage | `_code/D_DestroyerAPI/User/UserSecurityDalc.cs` `CreateUserSession` | Writes the session with a firm key (ACA/Code). For RSM both are the referring-site string, `Mcgdy` |
| Active count | Stored proc `dbo.D_GetFirmUserCount(aca, code, sessionTimeout)` | **Not in this repo**. Presumably counts active sessions by ACA/Code within `ApiSessionTimeoutInSeconds` (3600) |

`SeamlessLogin.aspx.cs` also exists in `MainUI_ethics` and `MainUI_coso`, but RSM uses
the root OPL app (`/mainui`). `ResourceSeamlessLogin.aspx.cs` mentions `MCGDY` only for
analytics labels. Its login dispatch has no McGladrey branch, so it isn't an RSM
entry point.

### Configuration

In `_code/MainUI/Web.config` (the same keys are copied into the ethics and COSO configs):

```xml
<add key="McgdyMaxUsers" value="100" />
<add key="McgdyascMaxUsers" value="100" />
```

The Web.config transforms don't override these, so production uses 100.

## What does and doesn't control access

- **There is no allow-list.** OPL admits any email RSM's portal sends. RSM's portals
  decide who is an RSM user.
- **The only limit is concurrent sessions:** 100 people logged in at the same time
  (a session counts as active until 60 minutes after its last activity,
  `ApiSessionTimeoutInSeconds` = 3600). It is **not** a cap on named
  users. Any number of people can rotate through the seats.
- **RSM staff and Alliance firm users share the same 100 seats.** Both portals send
  referring site `Mcgdy` and content domain `mcgladrey`, so OPL can't tell which portal a
  login came from. The **email domain** is the only thing that distinguishes the groups.
- Changing `McgdyascMaxUsers` does nothing today, because nobody logs in as `Mcgdyasc`.

## RSM vs external email domains

Classify `Mcgdy` users by the part of the email after the `@`.

**RSM (internal):**

| Domain | Notes |
|---|---|
| `rsmus.com` | Current RSM US staff |
| `mcgladrey.com` | Pre-2015 McGladrey addresses. Many people have a row under both this and `rsmus.com` |
| `rsm.pr` | RSM Puerto Rico. Treated as RSM in AIC-87; confirm with AICPA if it matters contractually |
| `rsm.ky` | RSM Cayman Islands |
| `rsmus.orb` | Typo of `rsmus.com` (2 accounts) |

Also on RSM domains: shared admin accounts such as `allianceadmin@rsmus.com`,
`advancecpaadmin@rsmus.com`, `firmfoundationadmin@rsmus.com`, `gamadmin@rsmus.com`.

**External (Alliance/AdvanceCPA firms): every other domain.** In Sept 2026, D_User held
about 1,970 external accounts across roughly 200 domains. The 37 firm domains active in
the last months before the cutoff (Sep to Nov 2024) were:

```
awhitney.com, bolluslynch.com, bstco.com, btco.cpa, ckpcpas.com, cpabr.com,
deandorton.com, dopkins.com, elliottdavis.com, hmvcpa.com, hogantaylor.com,
hoodstrong.com, horne.com, hsccpa.com, inserocpa.com, jacksonthornton.com,
kdpllp.com, kmhllp.com, kraftcpas.com, laporte.com, lb-cpa.com, lutz.us,
mgocpa.com, millercooper.com, pbmares.com, pkllp.com, pughcpas.com, reacpa.com,
sanvilleco.com, saxllp.com, singerlewak.com, srsnodgrass.com, tkw.com,
vasquezcpa.com, watsonrice.com, wolfandco.com, yhbcpa.com
```

**Noise to ignore:** `knowlysis.com` (the previous vendor's test accounts), `gmail.com` /
`gmal.com` test accounts, `a@a.org`, `a@b.com`, `ff.com`, `acpa.com`, and one blank email.

## History

| When | What happened |
|---|---|
| Years before 2024 | OPL migrated from `publication.cpa2biz.com` to `library.aicpa.org`. RSM never repointed their integration |
| 2024 | AICPA IT shut off `publication.cpa2biz.com`, which broke RSM's integration |
| Nov 6, 2024 | Last login by any external (Alliance) user. The PS+ client portal link has not worked since |
| Nov 2024 onward | RSM staff access continued (the employee link was restored separately; Kyle George helped RSM with it) |
| Apr to Jul 2025 | Jeff worked with RSM IT (Layton Coombs, Toby Burton, Christine Mattingly, Robert Alers, Jeremy Von Thun) to fix the PS+ link. Logs showed their system never called the token service or posted to SeamlessLogin. RSM never completed the change |
| May 30, 2025 | Some users were failing because they were stored under the AICPA store referring site instead of `Mcgdy`; Jeff corrected their records |
| Sep 2026 (AIC-87) | RSM told AICPA it will drop Alliance external users as of Nov 1, 2026, keeping 2 external users. Research in this doc done for Kyle George |

## Usage snapshot (Sep 2024 to Sep 2026)

- 133 distinct RSM staff in the 12 months ending Sep 23, 2026, usually 10 to 30 a month.
- Peak concurrency 5. No login ever refused by the 100 limit.
- External: 86 users from 37 firms between Sep 3 and Nov 6, 2024; 1,203 external logins
  Jan 2 to Nov 6, 2024; none since. In Sep 2024, external users outnumbered RSM staff
  about 3 to 1 (56 vs 17).

## Answering common questions (SQL)

Run against the OPL database. `D_User` columns: `UserId, LastLogin, CurrentSessionId,
LastSessionPoll, ReferringSite, Email, LicenseAgreement, Password, LastName, FirstName`
(FirstName/LastName are NULL for RSM users).

**Users by domain group, per month:**

```sql
DECLARE @From datetime = DATEADD(month, DATEDIFF(month, 0, GETDATE()) - 24, 0);

SELECT DATEADD(month, DATEDIFF(month, 0, e.EventTime), 0) AS [Month],
       COUNT(DISTINCT CASE WHEN g.UserGroup = 'RSM' THEN e.UserId END) AS RsmUsers,
       COUNT(DISTINCT CASE WHEN g.UserGroup <> 'RSM' THEN e.UserId END) AS ExternalUsers
FROM dbo.D_EventLog e
JOIN dbo.D_User u ON u.UserId = e.UserId
CROSS APPLY (SELECT CASE WHEN LOWER(SUBSTRING(u.Email, CHARINDEX('@', u.Email) + 1, 100))
                          IN ('rsmus.com', 'mcgladrey.com', 'rsm.pr', 'rsm.ky', 'rsmus.orb')
                     THEN 'RSM' ELSE 'External' END AS UserGroup) g
WHERE u.ReferringSite = 'Mcgdy'
  AND e.EventTime >= @From
GROUP BY DATEADD(month, DATEDIFF(month, 0, e.EventTime), 0)
ORDER BY [Month];
```

**Every login's raw parameters.** SeamlessLogin writes a `Name = 'Incoming Params'` row
for each attempt, with a Description like
`hidSecurityToken: ... hidUserId: ... hidReferringSite: Mcgdy hidDomain: mcgladrey hidRelayState: ...`.
These rows have no UserId, so filter on the Description text:

```sql
SELECT EventTime, Description
FROM dbo.D_EventLog
WHERE Name = 'Incoming Params'
  AND Description LIKE '%hidReferringSite: Mcgdy %'
ORDER BY EventTime DESC;
```

**Peak concurrency** is estimated, not measured. A user counts as active for 60 minutes
after any logged event, sampled every 15 minutes:

```sql
DECLARE @From datetime = DATEADD(month, DATEDIFF(month, 0, GETDATE()) - 24, 0);

WITH ev AS (
    SELECT e.UserId, e.EventTime
    FROM dbo.D_EventLog e
    JOIN dbo.D_User u ON u.UserId = e.UserId
    WHERE u.ReferringSite = 'Mcgdy' AND e.EventTime >= @From
), slots AS (
    SELECT ev.UserId,
           DATEADD(minute, 15 * (DATEDIFF(minute, 0, ev.EventTime) / 15 + k.n), 0) AS SlotTime
    FROM ev CROSS JOIN (VALUES (1), (2), (3), (4)) k(n)
), per_slot AS (
    SELECT SlotTime, COUNT(DISTINCT UserId) AS ActiveUsers
    FROM slots GROUP BY SlotTime
)
SELECT DATEADD(month, DATEDIFF(month, 0, SlotTime), 0) AS [Month],
       MAX(ActiveUsers) AS PeakConcurrentUsers
FROM per_slot
GROUP BY DATEADD(month, DATEDIFF(month, 0, SlotTime), 0)
ORDER BY [Month];
```

**Limit refusals** are logged two ways. A `Name = 'Firm Exceeded'` row is written only if
log settings allow it. The login page's error row is always written, with
`Description LIKE '%FIRM LIMIT EXCEEDED%'` and `Module = 'SessionExpired.aspx'` (the
config value is used as the module name), but it has no UserId.

## Options for limiting external users

External users can only get back in once RSM repoints their PS+ integration to
`library.aicpa.org`. After that:

1. **Separate pool of N concurrent users, config only.** RSM sets the PS+ portal to send
   referring site `Mcgdyasc`, and we set `McgdyascMaxUsers`. No code change. Not tested
   end to end.
2. **Separate pool of N concurrent users, code.** If RSM keeps sending `Mcgdy`, tag
   non-RSM email domains with their own firm key in `SetMcgdySubscriptionContext` and read
   a second max-users setting. Depends on `D_GetFirmUserCount` counting by ACA/Code.
3. **Named users, code.** Refuse `Mcgdy` logins whose email isn't an RSM domain or on a
   configured allow-list, with a clear error message (the existing one says the firm limit
   was exceeded).

Options 1 and 2 still admit any external user while fewer than N are logged in. Only
option 3 restricts access to specific people. Each code option is about half a day with
testing and deploy.

## Testing a login

`_code/RemoteAuth` is a test harness that simulates RSM's portal. Enter an email, choose
the referring site, click **Request Token** (calls the auth service), then **Login**
(posts to `SeamlessLoginPage`). **Its `SeamlessLoginPage` setting in
`RemoteAuth/Web.config` still points at `https://publication.cpa2biz.com/mainui/seamlesslogin.aspx`,
which no longer exists.** Point it at `https://library.aicpa.org/mainui/seamlesslogin.aspx`
before using it.

## Not verified

- The body of `D_GetFirmUserCount` (not in the repo).
- Whether IIS restricts access to `Authentication.svc` on the production server. The repo
  config uses transport security with no client credentials.
- The `Mcgdyasc` login path end to end.
- Exactly how RSM's employee link was restored after the 2024 domain shutoff.
