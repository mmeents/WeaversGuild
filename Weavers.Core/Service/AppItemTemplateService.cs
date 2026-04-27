using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Weavers.Core.Handlers.Templates;

namespace Weavers.Core.Service {

  public interface IAppItemTemplateService {
    Task<string?> GetSolutionTemplateCommand(int solutionItemId);
    Task<string?> GetDependencyInjectionTemplate(int classItemId);
    Task<string?> GetClassTemplate(int classItemId);
  }
  public class AppItemTemplateService(IServiceScopeFactory serviceScopeFactory) : IAppItemTemplateService {
    private readonly IServiceScopeFactory _scopeFactory = serviceScopeFactory;

    private IMediator GetMediator() {
      var scope = _scopeFactory.CreateScope();
      return scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public async Task<string?> GetSolutionTemplateCommand(int solutionItemId) { 
      var mediator = GetMediator();
      var result = await mediator.Send(new  GetSolutionTemplateCommand(solutionItemId));
      return result;
    }

    public async Task<string?> GetDependencyInjectionTemplate(int classItemId) {
      var mediator = GetMediator();
      var query = new GetDependencyInjectionTemplateCommand(classItemId);
      var result = await mediator.Send(query);
      return result;
    }

    public async Task<string?> GetClassTemplate(int classItemId) { 
      var mediator = GetMediator();
      var query = new GetClassTemplateCommand(classItemId);
      var result = await mediator.Send(query);
      return result;
    }




  }
}
