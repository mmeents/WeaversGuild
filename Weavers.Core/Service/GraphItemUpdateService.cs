using MediatR;
namespace Weavers.Core.Service {

  public interface IGraphItemUpdateService {
    void AddItem(int itemId);    
    event Action<int> OnItemAdded;
  }

  public class GraphItemUpdateService : IGraphItemUpdateService { 
    public GraphItemUpdateService() {  
    }
    public event Action<int> OnItemAdded = delegate { };
    private void RaiseItemAdded(int itemId) => OnItemAdded?.Invoke(itemId);
    public void AddItem(int itemId) => RaiseItemAdded(itemId);
  }

  public record ItemUpdatedNotification(int ItemId) : INotification;

  public class ItemUpdatedNotificationHandler : INotificationHandler<ItemUpdatedNotification> {
    private readonly IGraphItemUpdateService _service;
    public ItemUpdatedNotificationHandler(IGraphItemUpdateService service) { 
      _service = service; 
    }
    public Task Handle(ItemUpdatedNotification notification, CancellationToken ct) {
      _service.AddItem(notification.ItemId);
      return Task.CompletedTask;
    }
  }

}
