namespace NavigationHelper
{
    public interface INavigationRequestHandler<T>
    {
        public void HandleNavigation(T navigationRequest);
    }
}