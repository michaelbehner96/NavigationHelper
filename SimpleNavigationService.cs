namespace NavigationShark
{

    public class SimpleNavigationService<TNavigationTarget> : INavigationService<TNavigationTarget>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly HashSet<INavigationRequestHandler<TNavigationTarget>> handlers;
        public TNavigationTarget? CurrentLocation { get; set; }
        public List<TNavigationTarget> ForwardStack { get; set; } = new List<TNavigationTarget>();
        public List<TNavigationTarget> BackStack { get; set; } = new List<TNavigationTarget>();

        public SimpleNavigationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.handlers = new HashSet<INavigationRequestHandler<TNavigationTarget>>();
        }

        public void NavigateTo<T>() where T : TNavigationTarget
        {
            var navTarget = serviceProvider.GetService(typeof(T));

            if (navTarget == null)
                throw new InvalidOperationException($"Failed getting navigation target of type {typeof(T)} from the service provider.");

            var destination = (TNavigationTarget)navTarget;

            ForwardStack.Clear();

            if (CurrentLocation != null)
                BackStack.Add(CurrentLocation);

            CurrentLocation = destination;
            BroadcastNavigationRequest(CurrentLocation);
        }

        public bool NavigateBack()
        {
            if (BackStack.Count == 0)
                return false;

            if (CurrentLocation != null)
                ForwardStack.Add(CurrentLocation);

            CurrentLocation = BackStack.Last();
            BackStack.RemoveAt(BackStack.Count - 1);
            BroadcastNavigationRequest(CurrentLocation);

            return true;
        }

        public bool NavigateForward()
        {
            if (ForwardStack.Count == 0)
                return false;

            if (CurrentLocation != null)
                BackStack.Add(CurrentLocation);

            CurrentLocation = ForwardStack.Last();
            ForwardStack.RemoveAt(ForwardStack.Count - 1);
            BroadcastNavigationRequest(CurrentLocation);

            return true;
        }

        private void BroadcastNavigationRequest(TNavigationTarget navigationTarget)
        {
            foreach (var handler in handlers)
            {
                handler.HandleNavigation(navigationTarget);
            }
        }

        public bool RegisterHandler(INavigationRequestHandler<TNavigationTarget> requestHandler)
        {
            return handlers.Add(requestHandler);
        }

        public bool UnregisterHandler(INavigationRequestHandler<TNavigationTarget> requestHandler)
        {
            return handlers.Remove(requestHandler);
        }
    }
}