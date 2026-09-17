using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace MainUI.Shared
{
    /// <summary>
    /// Builds cache-busting URLs for static assets. js and css are served with a
    /// one-year max-age (staticContent/clientCache in Web.config), so their URLs
    /// have to change when the file changes or browsers keep the old copy.
    /// </summary>
    public static class Assets
    {
        /// <summary>
        /// Resolves an app-relative path the way ResolveUrl does, then appends a
        /// version stamp taken from the file's last-write time. Static assets only;
        /// routes and form actions keep ResolveUrl.
        /// </summary>
        public static string VersionedUrl(string virtualPath)
        {
            string url = VirtualPathUtility.ToAbsolute(virtualPath);

            string physical = HostingEnvironment.MapPath(virtualPath);
            if (physical == null || !File.Exists(physical))
                return url;

            string cacheKey = "assetver:" + virtualPath;
            string stamp = HttpRuntime.Cache[cacheKey] as string;
            if (stamp == null)
            {
                // Hex ticks rather than a hash, so the stamp decodes back to a real
                // timestamp when working out which copy a browser is holding.
                stamp = File.GetLastWriteTimeUtc(physical).Ticks.ToString("x");

                // CacheDependency, not a static dictionary: replacing a .js in the
                // app folder does not recycle the app pool (only bin, App_Code and
                // Web.config do), so a static cache would serve the old stamp until
                // the next recycle. Partial deploys are the normal path here.
                HttpRuntime.Cache.Insert(cacheKey, stamp, new CacheDependency(physical));
            }

            return url + (url.IndexOf('?') >= 0 ? "&" : "?") + "v=" + stamp;
        }
    }
}
