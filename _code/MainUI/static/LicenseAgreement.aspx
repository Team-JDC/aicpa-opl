<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LicenseAgreement.aspx.cs" Inherits="MainUI.LicenseAgreement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AICPA License Agreement</title>
	<style>
		.la-container {margin-left:20px; padding-top:20px; }
		DIV.license-agreement { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px }
		DIV.license-agreement H1 { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 5px; MARGIN: 0px 0px 0px 20px; FONT: 28px Arial, Helvetica, sans-serif; PADDING-TOP: 0px }
		DIV.license-agreement H2 { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px 0px 0px 20px; FONT: 18px Arial, Helvetica, sans-serif; PADDING-TOP: 0px }
		DIV.license-agreement H3 { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; FONT: bold 14px Arial, Helvetica, sans-serif; PADDING-TOP: 0px }
		DIV.license-agreement OL { PADDING-RIGHT: 0px; PADDING-LEFT: 35px; PADDING-BOTTOM: 10px; MARGIN: 0px; FONT: 14px "Times New Roman", Times, serif; PADDING-TOP: 10px; LIST-STYLE-TYPE: upper-roman }
		DIV.license-agreement OL LI { FONT: 12px Arial, Helvetica, sans-serif }
		DIV.license-agreement OL LI OL { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; MARGIN: 0px; PADDING-TOP: 10px; LIST-STYLE-TYPE: none }
		DIV.license-agreement OL LI OL LI { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px }
		DIV.warning { BORDER:2px solid #000; PADDING: 10px; TEXT-ALIGN: left; background-color: #ccc } 
		DIV.warning P { FONT: 12px Arial } 
	</style>    
    <script type="text/javascript" src="/js/jquery/jquery-1.11.2.min.js"></script>    
    <script type="text/javascript" src="/js/jquery.cookie.js"></script>
    <script>
        function disagreeLA()
        {
            parent.callWebService("/WS/DocumentTools.asmx/UpdateLAStatus", "{newVal:false}", disagreeCallback, parent.ajaxFailed);
        }

        function disagreeCallback()
        {
                    $('#warningBanner').show();            
        }

        function agreeLA()
        {
            parent.callWebService("/WS/DocumentTools.asmx/UpdateLAStatus", "{newVal:true}", agreeCallback, parent.ajaxFailed);
        }

        function agreeCallback()
        {
            window.location.reload();
           // parent.doLALink(<%= NodeId %>, '<%= NodeType %>');
        }
        $().ready(function () {
            $("#rightcol").hide();
        });
    </script>
</head>
<body>

<div id="leftcol" class="col-sm-12">
    <div class="leftcol_inner">
        <div class="leftcol_header">
            <h1>License Agreement</h1>
        </div>
        <div class="leftcol_content">


<form id="form1" runat="server">
<div class="la-container">
    <div class="warning" id="warningBanner" style="display: none">
		<p>The license agreement must be accepted to view content of the Financial 
			Accounting Standards Board (FASB).</p>
		<p>If you do not wish to accept the agreement, you may continue working in the 
			non-FASB titles of your subscription now by selecting a title from the table of 
			contents.</p>
		<p>Please contact the AICPA Member Service Center at <a href="mailto:service@aicpa.org"><b>service@aicpa.org</b></a> or by 
			calling <b>888.777.7077</b></p>
	</div>
	<div class="license-agreement">
		<div style="height: 550px; border: 1px solid black; width: 95%; overflow-y: scroll; padding:15px; background:#f9f8f8;">
		<h1>License Agreement Required</h1>
		<h2>Sub-License Agreement for Access to Content Via AICPA Online Professional Library</h2>
		<ol>
			<li>
				<h3>Scope of Agreement</h3>
				<p>Pursuant to this Agreement, AICPA has agreed to provide, and you ("Licensee") 
					have agreed to subscribe to certain online content, information, and research 
					tools (the "Library") solely as provided for herein. The Agreement allows 
					Licensee access to the library for itself and (in the case of multiple user 
					licenses) a designated subset of sub-licensees (“end users”). This Sublicense 
					defines the terms and conditions of access to the Library by Licensee and/or 
					any additional end users. Licensee understands that AICPA is providing the 
					Library to Licensee as well as to other customers, and that access to the 
					Library is non-exclusive and (accept in the case of sub-licensure as provided 
					for under the Agreement) non-transferable.</p>
			</li>
            <li>
				<h3>The Library</h3>
				<p>Throughout the term of this Agreement, Licensee and Licensee’s end users shall 
					have access to the Library. AICPA may, without advance notice or liability, 
					add, discontinue, or revise any aspect of the Library, including without 
					limitation such aspects as scope, time and availability of information. The 
					Library may only be used for lawful purposes. Providing of content and use of 
					any information obtained through the Library is at Licensee and any end user’s 
					own risk and AICPA specifically disclaims any liability, warranty or 
					responsibility for the accuracy, correctness, timeliness or quality of the 
					information and content provided or obtained through such use of the Library 
					and for Licensee or any end user’s reliance upon the Library.</p>
			</li>
            <li>
				<h3>Title</h3>
		<ol>
			<li>
			Title, ownership rights, and any and all intellectual property rights in and to 
			the materials contained in the Library (and any and all copies thereof, 
			tangible or intangible) shall remain with AICPA at all times. The materials 
			contained in the Library are protected by copyright laws and international 
			copyright treaties.
			</li>
            <li>
			Certain portions of the Library may consist of data, services, and other 
			materials proprietary to third parties which have licensed to AICPA the right 
			to redistribute or sublicense such materials. Such third party licensors shall 
			be third party beneficiaries of this Agreement to the fullest extent allowed by 
			law. As between AICPA and Licensee, AICPA shall have and retain all title and 
			ownership of, and intellectual property and other rights in and to, the Library 
			and all included materials, together with all copies, Updates, Upgrades, new 
			versions, and any other manifestations thereof. AICPA reserves all rights not 
			expressly granted to Licensee under this Agreement. No intellectual property 
			right (including, without limitation, all copyrights, program, or database 
			structure and organization, specific sets of information extracted therefrom, 
			non-public data, and specifics about the means and standards of compilation) 
			shall vest in or be transferred to Licensee.
			</li>
            <li>
				If Licensee subscribes to a Library with the designation “Plus FASB Accounting 
				Standards Codification&trade;” in the Library name, then Licensee specifically 
				acknowledges that the Library includes the FASB Accounting Standards 
				Codification&trade; (FASB ASC&trade;) in its entirety and that with respect to the FASB 
				ASC&trade; content, all of the rights, benefits, warranties, indemnifications and 
				limitations on liability granted to AICPA by Licensee enure to the full benefit 
				of the Financial Accounting Foundation (FAF) and may be enforced by AICPA on 
				behalf of FAF to the fullest extent that would be allowed under the Agreement 
				or this Sublicense for content owned by and copyrighted to AICPA. AICPA 
				represents and warrants that has the right, pursuant to its Agreement with FAF, 
				to extend access to FASB ASC within the Library pursuant to this Sublicense. 
				The FASB ASC is a "commercial item" as that term is defined in 48 CFR 12.101 
				(Oct. 1995), consisting of "commercial computer software" and "commercial 
				computer software documentation" as such terms are used in 48 CFR 12.212 (Sept. 
				1995). Consistent with 48 CFR 12.212 and 48 CFR 227.7202-1 through 227.7202-4 
				(June 1995), all U.S. Government end users acquire the Licensed Materials with 
				only those rights explicitly set forth herein.</li>
		</ol>
		<li>
			<h3>Limits of Permissible Use.</h3>
			<ol>
		<li>
		Licensee and its end users shall not: (i) permit other Persons to use, access, 
		distribute, or display the content accessed via the Library except under the 
		terms set forth herein; (ii) modify, translate, reverse engineer, decompile, 
		disassemble, or create derivative works based on the content contained within 
		the Library or any portion thereof; (iii) copy content contained within the 
		Library (except as expressly set forth herein); or (iv) remove any proprietary 
		notices or labels included within or imprinted as a part of the content 
		contained within the Library.
		</li>
        <li>
			Notwithstanding anything to the contrary contained herein, Licensee shall not 
			have the right to (i) resell the content contained within the Library or grant 
			any licenses to the Library (except for sublicenses in compliance with the 
			Agreement); (ii) use the Library in any service bureau or time sharing 
			arrangement; (iii) make the Library or the content contained within the Library 
			available as an application service provider, (iv) use the content contained 
			within the Library other than as a reference source; or (v) re-engineer, 
			repurpose, reconfigure or modify the content contained within the Library in 
			such a manner as to alter its substance, meaning or intent.</li>
		</ol>
		<p>Subscription to or purchase of access to, the Library grants permission ONLY to 
			view the material and save the material ONLY for the Licensee or any end user's 
			personal reading, but NOT to further copy, modify, use or distribute in any way 
			or create any derivative works except as specifically authorized below. Neither 
			Licensee nor any end user may remove any copyright or trademark notices, such 
			as the &copy;, &trade; or &reg; symbols, from the content of the Library or any web page or 
			interface through which the Library is accessed.</p>
		</li>
        <li>
			<h3>End User Requirements</h3>
			<p>Licensee acknowledges that the information provided in connection with the 
				Library contains copyrighted and other proprietary and confidential information 
				and material, and will respect all such proprietary rights and take such 
				reasonable precautions to protect such information and material from 
				unauthorized use or disclosure. Licensee further agrees it shall not violate 
				any laws, regulations or standards established by an entity of competent 
				jurisdiction relating to the promotion or providing of the Library. Licensee 
				undertakes full responsibility for communicating the requirements of this 
				Sublicense Agreement to its end users and enforcing the compliance of end users 
				with all terms of this Sublicense Agreement. Licensee will require each end 
				user to accept and acknowledge a use agreement designed to enforce full 
				compliance by the end user with the terms and conditions of this Sublicense.</p>
		</li>
        <li>
			<h3>Term and Termination</h3>
			<p>This Sublicense shall have a term of one (1) year, or, if the Licensee renews 
				its Agreement with AICPA this Sublicense shall have a longer term equal to the 
				Agreement renewal period. The Sublicense shall immediately terminate at any 
				time upon notice from AICPA if Licensee or any end user fails to comply with 
				these terms and conditions. Licensee acknowledges and agrees that termination 
				of this Sublicense for any reason during the initial term of a subscription 
				will, at AICPA’s discretion, result in pro rata charges according to the number 
				of months remaining in the initial term of the Agreement to which this 
				Sublicense pertains. Termination for any reason will not relieve Licensee from 
				its obligation to pay AICPA all sums owed through the effective date of 
				termination.</p>
			<ol>
			<li>
			Upon any termination or expiration of this Agreement, Licensee agrees to (i) 
			discontinue use and to destroy all copies and delete all instances of the 
			Library Content, specifically including the FASB Accounting Standards 
			Codification&trade; (existing in whole and in part) from the networks, servers of 
			Licensee; and (ii) notify all end users; provided, that, in the case of a 
			termination other than for breach of the terms of the Agreement and this 
			Sublicense, end users who are in full compliance with the Sublicense terms may 
			continue to use the Library until the earlier of (a) the termination of the 
			then-current term of such end user’s agreement with Licensee regarding the 
			Library, or (b) the first anniversary of the Effective Termination Date or 
			expiration of the Agreement.</li>
		</ol>
		</li>
        <li>
			<h3>Limitations on Warranty</h3>
			<p>AICPA MAKES NO WARRANTIES OF ANY KIND WITH RESPECT TO THE RELIABILITY OF THE 
				LIBRARY UNDER THE TERMS OF THIS AGREEMENT OR THE FITNESS OF SUCH LIBRARY FOR A 
				PARTICULAR PURPOSE AND DISCLAIMS ALL EXPRESS AND IMPLIED WARRANTIES, INCLUDING 
				MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE, AND ANY IMPLIED WARRANTY 
				OF NON-INFRINGEMENT. IN NO EVENT WILL AICPA BE LIABLE FOR ANY LOSS OF PROFITS, 
				BUSINESS, INCIDENTAL OR CONSEQUENTIAL DAMAGES OF ANY KIND, WHETHER BASED IN 
				CONTRACT, NEGLIGENCE OR OTHER TORT. USE OF THE LIBRARY IS AT LICENSEE AND END 
				USER'S SOLE RISK. THE LIBRARY IS PROVIDED “AS IS.”</p>
		</li>
        <li>
			<h3>Indemnification</h3>
			<p>Licensee shall defend and indemnify AICPA from all claims, suits, damages and 
				costs (including attorneys' and experts' fees) arising out of Licensee and any 
				End User's use of the Library or Licensee or any End User's breach of this 
				Agreement.</p>
		</li>
        <li>
			<h3>Limitation of Liability</h3>
			<ol>
		<li>
		Under no circumstances shall either party's liability to the other for the 
		failure or asserted failure of such party to perform its obligations hereunder 
		include, nor shall such party be liable for, special, incidental, 
		consequential, or tort damages, including, without limitation, punitive damages 
		and damages resulting from delay or from loss of profits, business or goodwill, 
		whether or not such party has been advised or is aware of the possibility of 
		such damages.</li>
		<li>
			Under no circumstances shall AICPA’s aggregate liability to Licensee arising 
			out of or related to this Agreement exceed the lesser of (i) the aggregate fees 
			paid to AICPA by Licensee hereunder during the preceding twelve (12) month 
			period or (ii) the actual damages sustained by Licensee, regardless of whether 
			any action or claim is based on warranty, contract, tort or otherwise. Licensee 
			hereby releases Licensor from all obligations, liability, claims or demand in 
			excess of this limitation.</li>
		</ol>
		<li>
			<h3>Confidentiality</h3>
			<p>Other than as may be required by any applicable law, government order or 
				regulation, or by order or decree of any court of competent jurisdiction, the 
				parties shall not publicly divulge or announce, or in any manner disclose to 
				any third party, any confidential information revealed to the parties pursuant 
				hereto, or any of the specific terms and conditions of this Agreement.</p>
			<p>Licensee acknowledges that AICPA treats: (i) the terms of the transaction 
				contemplated herein, the delivery, methodology, platforms, pricing, and (ii) 
				all other information and components of the delivery of the Library and the 
				terms of this Agreement (and the obligations hereunder), not generally 
				available to the public, in each case, as AICPA's confidential information, 
				whether or not particular portions or aspects thereof may also be available 
				from other sources. Licensee will likewise take reasonable measures to protect 
				the secrecy of and avoid disclosure and unauthorized use of AICPA’s 
				confidential information. Such measures will be no less stringent than the 
				measures that Licensee takes to protect its own most highly confidential 
				business information. Licensee acknowledges that unauthorized disclosure or use 
				of confidential information could cause irreparable harm to AICPA for which 
				monetary damages alone would not be a sufficient remedy. AICPA will have the 
				right, in addition to its other rights and remedies, to seek injunctive relief 
				for or to prevent any unauthorized disclosure or use, and to limit or recover 
				any improper benefits derived therefrom.</p>
		</li>
        <li>
			<h3>Governing Law</h3>
			<p>This Agreement shall be governed by, and construed and interpreted in accordance 
				with, the laws of the United States of America and the State of New York, 
				without reference to the principles of conflicts of laws.</p>
		</li>
        <li>
			<h3>Disputes</h3>
			<ol>
            	<li>
                Any controversies or disputes arising out of or relating to this Agreement 
                shall be resolved by binding arbitration in accordance with the then current 
                “Commercial Arbitration Rules” of the “American Arbitration Association.” The 
                parties shall endeavor to select a mutually acceptable arbitrator knowledgeable 
                about issues relating to the subject matter of this Agreement. In the event the 
                parties are unable to agree to such a selection, each party will select an 
                arbitrator and the arbitrators in turn shall select a third arbitrator. The 
                arbitration shall take place in New York.</li>
            </ol>
            <p>All documents, materials, and information in the possession of each party that 
                are in any way relevant to the claim(s) or dispute(s) shall be made available 
                to the other party for review and copying no later than ten days after the 
                notice of arbitration is served.</p>
            <p>The arbitrator(s) shall not have the authority, power, or right to alter, 
                change, amend, modify, add, or subtract from any provision of this Agreement or 
                to award special, incidental, consequential, or tort damages, including, 
                without limitation, punitive damages and damages resulting from delay or from 
                loss of profits, business or goodwill, whether or not such party has been 
                advised or is aware of the possibility of such damages. The arbitrator shall 
                have the power to issue mandatory orders and restraining orders in connection 
                with the arbitration. The award rendered by the arbitrator shall be final and 
                binding on the parties, and judgment may be entered thereon in any court having 
                jurisdiction. The agreement to arbitration shall be specifically enforceable 
                under prevailing arbitration law. During the continuance of any arbitration 
                proceeding, except as otherwise set forth herein, the parties shall, without 
                delay, continue to perform their respective obligations under this Agreement 
                which are not affected by the dispute. The foregoing shall not be deemed to 
                limit all remedies and rights available to either party as otherwise provided 
                herein.</p>
		</li>
        <li>
			<h3>Assignment</h3>
			<p>Except with respect to the conveyance of access to the Library for an End User 
				by Licensee under the express terms of the Agreement and this Sub-License, 
				neither Licensee nor any End User shall transfer or assign this Agreement or 
				any rights or obligations hereunder without the prior, written approval of 
				AICPA, which may be withheld at the sole discretion of AICPA and any assignment 
				in violation of this prohibition shall be void. Subject to the foregoing, this 
				Agreement shall be binding upon and inure to the benefit of the parties hereto, 
				their successors and assigns</p>
		</li>
        <li>
			<h3>Waiver</h3>
			<p>The waiver by either party of a breach of any provision of this Agreement shall 
				not operate or be construed as a continuing waiver of or consent to any 
				subsequent breach.</p>
		</li>
        <li>
			<h3>Interruptions in Access</h3>
			<p>To the extent AICPA makes the Library accessible via the Internet, AICPA 
				anticipates that such access would be available to Licensee and End Users on a 
				24-hour, 7-day-per-week basis. Notwithstanding the foregoing, AICPA does not, 
				and cannot, guarantee that the Library will be available for access on a 
				24-hour, 7-day-per-week basis and Licensee acknowledges and agrees to the same. 
				In addition to downtime caused by reasons beyond AICPA’s control, Licensee 
				understands that AICPA may interrupt access for normal and customary 
				maintenance, to correct errors or remedy problems, to implement Updates, if 
				any, and at other times as deemed necessary or desirable by AICPA. Licensee 
				acknowledges that its inability to access the Library from time to time during 
				the Term is to be expected, and shall not constitute a breach of this Agreement 
				by AICPA. In addition, Licensee agrees that AICPA shall have no liability to 
				Licensee or any other Person (including Sublicensees) for any damages or harm 
				such Person may suffer as a result of the inability to access the Licensed 
				Materials. Moreover, Licensee hereby releases AICPA from any and all claims 
				Licensee may have against AICPA or its agents or 3rd party beneficiaries as a 
				result of the inability to access the Licensed Materials. Licensee shall 
				indemnify and hold harmless the AICPA from and against any and all claims or 
				damages that AICPA may incur as a result of any claims brought by Licensee or 
				any sublicensee.</p>
		</li>
        <li>
			<h3>Audit Rights</h3>
			<p>During the Term and for an additional two (2) years thereafter, AICPA shall have 
				the right to inspect, or have a reputable third party inspect, Licensee’s data 
				processing systems and records for the sole purpose of verifying that 
				Licensee’s use is consistent with the terms of this Agreement. Such inspections 
				will be made not more than once annually and on not less than ten (10) Business 
				Days written notice, during regular business hours. If the inspection reveals 
				an underpayment to AICPA, the Licensee shall promptly (and, in any event, 
				within ten (10) Business Days of such determination) pay to AICPA the deficit 
				plus commercially reasonable interest. AICPA shall bear the expense of such 
				inspection unless the inspection reveals the underpayment of license fees that 
				vary more than five percent (5%) from the license fees received by AICPA from 
				Licensee, in which case the Licensee shall bear the costs associated with the 
				inspection. If the inspection reveals an overpayment to AICPA, then AICPA shall 
				promptly (and, in any event, within ten (10) Business Days of such 
				determination) pay to Licensee the overpayment amount. Audit personnel, when on 
				Licensee’s premises or accessing Licensee’s networks or performing audit 
				services hereunder, will be instructed to comply with all of Licensee’s 
				security, supervision, and other standard procedures applicable to such 
				representatives so long as Licensee shall have made such applicable procedures 
				available to such representatives at least three (3) Business Days prior to any 
				inspection or notice.</p>
		</li>
        <li>
			<h3>Validity</h3>
			<p>The invalidity or unenforceability of any provision of this Agreement shall not 
				affect the validity or enforceability of any other provisions of this 
				Agreement, which shall remain in full force and effect.</p>
		</li>
		</ol>
		</div>
        <br />
        <a href="#" onclick="javascript:agreeLA();"><img src="/images/btn-agree.gif" alt="Agree" border="0" /></a>
		<a href="#" onclick="javascript:disagreeLA();"><img src="/images/btn-disagree.gif" alt="Disagree" border="0" style="margin-left:5px;" /></a>
    </div>
</div>
</form>
        </div>
    </div>
</div>
</body>
</html>
