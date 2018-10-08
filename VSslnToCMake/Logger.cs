/*-----------------------------------------------------------------------------
 * Copyright (c) DaisukeAtaraxiA. All rights reserved.
 * Licensed under the MIT License.
 * See LICENSE.txt in the project root for license information.
 *---------------------------------------------------------------------------*/

namespace VSslnToCMake
{
    public abstract class Logger
    {
        public virtual void Info(string message)
        {
            WriteLine(message);
        }

        public virtual void Warn(string message)
        {
            WriteLine(message);
        }

        public virtual void Error(string message)
        {
            WriteLine(message);
        }

        abstract protected void WriteLine(string message);
    }
}
