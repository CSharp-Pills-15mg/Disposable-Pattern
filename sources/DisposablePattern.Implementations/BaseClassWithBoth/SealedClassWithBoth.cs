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
    public sealed class SealedClassWithBoth : IDisposable
    {
        private bool isDisposed;

        private readonly IntPtr pointer;
        private readonly MemoryStream memoryStream;

        public SealedClassWithBoth()
        {
            pointer = Marshal.AllocHGlobal(1024);
            memoryStream = new MemoryStream();
        }

        public void DoSomeWork()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName, "The current instance was disposed.");

            // Use the pointer and/or the memory stream.
            // ...
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
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
        }

        // No protected virtual method because there are no inheritors possible.

        // Finalizer added because we have an unmanaged resource (the pointer).
        ~SealedClassWithBoth()
        {
            Dispose(false);
        }
    }
}