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

namespace DustInTheWind.DisposablePattern.Implementations.BaseClassWithManaged
{
    public sealed class SealedClassWithManaged : IDisposable
    {
        private bool isDisposed;

        private readonly MemoryStream memoryStream;

        public SealedClassWithManaged()
        {
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
            if (isDisposed)
                return;

            // Dispose any disposable resources here.
            memoryStream.Dispose();

            isDisposed = true;

            // No finalization suppressed. There will never be a finalizer.
        }

        // No protected virtual method because there are no inheritors possible.

        // No finalizer method needed.
    }
}