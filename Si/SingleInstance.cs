#region License

/* Copyright (c) 2016 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Collections.Generic;

namespace Si
{
    /// <summary>
    /// Enforces single instance for an application.
    /// </summary>
    public class SingleInstance : IDisposable
    {
        private Mutex mutex = null;
        private Boolean ownsMutex = false;
        private Guid identifier = Guid.Empty;

        /// <summary>
        /// Enforces single instance for an application.
        /// </summary>
        /// <param name="identifier">An identifier unique to this application.</param>
        public SingleInstance(Guid identifier)
        {
            this.identifier = identifier;
            mutex = new Mutex(true, identifier.ToString(), out ownsMutex);
        }

        /// <summary>
        /// Indicates whether this is the first instance of this application.
        /// </summary>
        public Boolean IsFirstInstance
        { get { return ownsMutex; } }

        /// <summary>
        /// Passes the given arguments to the first running instance of the application.
        /// </summary>
        /// <param name="arguments">The arguments to pass.</param>
        /// <returns>Return true if the operation succeded, false otherwise.</returns>
        public Boolean PassArgumentsToFirstInstance(String[] arguments)
        {
            if (IsFirstInstance)
                throw new InvalidOperationException("This is the first instance.");

            try
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream(identifier.ToString()))
                using (StreamWriter writer = new StreamWriter(client))
                {
                    client.Connect(200);

                    foreach (String argument in arguments)
                        writer.WriteLine(argument);
                }
                return true;
            }
            catch (TimeoutException)
            { } //Couldn't connect to server
            catch (IOException)
            { } //Pipe was broken

            return false;
        }

        /// <summary>
        /// Listens for arguments being passed from successive instances of the applicaiton.
        /// </summary>
        public void ListenForArgumentsFromSuccessiveInstances()
        {
            if (!IsFirstInstance)
                throw new InvalidOperationException("This is not the first instance.");
            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenForArguments));
        }

        /// <summary>
        /// Listens for arguments on a named pipe.
        /// </summary>
        /// <param name="state">State object required by WaitCallback delegate.</param>
        private void ListenForArguments(Object state)
        {
            try
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream(identifier.ToString()))
                using (StreamReader reader = new StreamReader(server))
                {
                    server.WaitForConnection();

                    List<String> arguments = new List<String>();
                    while (server.IsConnected)
                        arguments.Add(reader.ReadLine());

                    ThreadPool.QueueUserWorkItem(new WaitCallback(CallOnArgumentsReceived), arguments.ToArray());
                }
            }
            catch (IOException)
            {
                Console.WriteLine("\nError IOException SingleInstance");
            } //Pipe was broken
            finally
            {
                ListenForArguments(null);
            }
        }

        /// <summary>
        /// Calls the OnArgumentsReceived method casting the state Object to String[].
        /// </summary>
        /// <param name="state">The arguments to pass.</param>
        private void CallOnArgumentsReceived(Object state)
        {
            OnArgumentsReceived((String[])state);
        }
        /// <summary>
        /// Event raised when arguments are received from successive instances.
        /// </summary>
        public event EventHandler<ArgumentsReceivedEventArgs> ArgumentsReceived;
        /// <summary>
        /// Fires the ArgumentsReceived event.
        /// </summary>
        /// <param name="arguments">The arguments to pass with the ArgumentsReceivedEventArgs.</param>
        private void OnArgumentsReceived(String[] arguments)
        {
            if (ArgumentsReceived != null)
                ArgumentsReceived(this, new ArgumentsReceivedEventArgs() { Args = arguments });
        }

        #region IDisposable
        private Boolean disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (mutex != null && ownsMutex)
                {
                    try
                    {
                        mutex.ReleaseMutex();
                        mutex = null;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        mutex = null;
                    }
                    
                }
                disposed = true;
            }
        }

        ~SingleInstance()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
