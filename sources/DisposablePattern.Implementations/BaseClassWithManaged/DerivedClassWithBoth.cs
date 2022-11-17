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

namespace DustInTheWind.DisposablePattern.Implementations.BaseClassWithManaged
{
    public class DerivedClassWithBoth : BaseClassWithManaged
    {
        private bool isDisposed;

        private readonly IntPtr pointer;
        private readonly MemoryStream memoryStream;

        public DerivedClassWithBoth()
        {
            pointer = Marshal.AllocHGlobal(1024);
            memoryStream = new MemoryStream();
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
                memoryStream.Dispose();
            }

            // Free any unmanaged resources here.
            Marshal.FreeHGlobal(pointer);

            isDisposed = true;

            // Call the base class implementation.
            base.Dispose(isDisposing);
        }

        // Finalizer method added because the base class does not provide one and we have unmanaged resources here.
        ~DerivedClassWithBoth()
        {
            Dispose(false);
        }
    }
}