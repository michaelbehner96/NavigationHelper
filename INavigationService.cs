namespace NavigationShark
{
    public interface INavigationService<TNavigationTarget>
    {
        public bool RegisterHandler(INavigationRequestHandler<TNavigationTarget> requestHandler);

        public bool UnregisterHandler(INavigationRequestHandler<TNavigationTarget> requestHandler);

        public void NavigateTo<T>() where T : TNavigationTarget;

        public bool NavigateBack();

        public bool NavigateForward();
    }
}