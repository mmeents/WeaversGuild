using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;

namespace Weavers.Core.Handlers.Todo {
  public record ReviewFailCommand(int TodoId, string ReviewNotes, string ChangeRequest) : IRequest<string>;
  internal class ReviewFailCommandHandler : IRequestHandler<ReviewFailCommand, string> {
    public Task<string> Handle(ReviewFailCommand request, CancellationToken cancellationToken) {
      // Implement the logic for handling the ReviewFailCommand here
      throw new NotImplementedException();
    }
  }
}
