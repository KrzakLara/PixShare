using System;
using System.Collections.Generic;

namespace PixShareLIB.Observer
{
    public class UserActions : ISubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string action, string user)
        {
            foreach (var observer in _observers)
            {
                observer.Update(action, user);
            }
        }

        public void PerformAction(string action, string user)
        {
            // Perform the action
            ExecuteAction(action, user);

            // Notify observers
            Notify(action, user);
        }

        private void ExecuteAction(string action, string user)
        {
            // Here you can add the logic for different actions
            switch (action)
            {
                case "Login":
                    Console.WriteLine($"{user} logged in.");
                    // Add logic for user login
                    break;

                case "UploadPhoto":
                    Console.WriteLine($"{user} uploaded a photo.");
                    // Add logic for uploading a photo
                    break;

                case "ChangePackage":
                    Console.WriteLine($"{user} changed their package.");
                    // Add logic for changing user package
                    break;

                // Add other actions as needed
                default:
                    Console.WriteLine($"{user} performed an unknown action: {action}");
                    break;
            }
        }
    }
}
