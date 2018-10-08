/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

namespace VSslnToCMake
{
    public class StdLogger : Logger
    {
        public override void Warn(string message)
        {
            base.Warn("Warning: " + message);
        }
        public override void Error(string message)
        {
            base.Error("Error: " + message);
        }

        protected override void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
