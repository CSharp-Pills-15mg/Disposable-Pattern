// C# Pills 15mg
// Copyright (C) 2019-2022 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Runtime.InteropServices;

namespace DustInTheWind.DisposablePattern.Implementations.BaseClassWithBoth
{
    public class DerivedClassWithBoth : BaseClassWithBoth
    {
        private bool isDisposed;

        private readonly IntPtr pointerInDerived;
        private readonly MemoryStream memoryStreamInDerived;

        public DerivedClassWithBoth()
        {
            pointerInDerived = Marshal.AllocHGlobal(1024);
            memoryStreamInDerived = new MemoryStream();
        }

        public void DoSomeWorkFromDerived()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName, "The current instance was disposed.");

            // Use the pointer and/or the memory stream.
            // ...
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposed)
                return;

            if (isDisposing)
            {
                // Dispose any disposable resources here.
                memoryStreamInDerived.Dispose();
            }

            // Free any unmanaged resources here.
            Marshal.FreeHGlobal(pointerInDerived);

            isDisposed = true;

            // Call the base class implementation.
            base.Dispose(isDisposing);
        }

        // No finalizer needed. The base class provides one.
    }
}