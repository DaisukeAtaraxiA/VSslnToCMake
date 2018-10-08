/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSslnToCMake
{
    sealed class Utility
    {
        private Utility() {}

        public static string NormalizePath(string path)
        {
            return new System.IO.FileInfo(path).FullName;
        }

        public static string ToRelativePath(string path, string dir)
        {
            if (dir == "")
            {
                return "";
            }
            string dir2 = dir;
            if (dir.Last() != '\\' && dir.Last() != '/')
            {
                dir2 = dir + '\\';
            }
            var uri = new Uri(dir2);
            return uri.MakeRelativeUri(new Uri(path)).ToString();
        }

        public static string ToUnixPath(string path)
        {
            var path2 = path.Replace('\\', '/');
            return path2.TrimEnd('/');
        }

        public static string AddTrailingSlashToPath(string path)
        {
            if (path.Length > 0 && path.Substring(path.Length - 1) != "/")
            {
                return path + "/";
            }
            else
            {
                return path;
            }
        }

        public static string AddDoubleQuotesToPath(string path)
        {
            if (path.Contains(" "))
            {
                return "\"" + path + "\"";
            }
            else
            {
                return path;
            }
        }
    }
}
