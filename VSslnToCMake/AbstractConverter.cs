/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

namespace VSslnToCMake
{
    public abstract class AbstractConverter
    {
        /// <summary>
        /// Target platform
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Target configurations
        /// </summary>
        public string[] TargetConfigurations { get; set; }

        protected Logger logger = new NullLogger();

        public void SetLogger(Logger logger)
        {
            this.logger = logger;
        }

        public abstract bool Convert(EnvDTE.DTE dte);
    }
}
