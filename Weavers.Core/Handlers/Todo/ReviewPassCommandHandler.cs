using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Weavers.Core.Constants;

namespace Weavers.Core.Handlers.Todo {
  public record ReviewPassCommand(int TodoId, string ReviewNotes) : IRequest<string>;

  internal class ReviewPassCommandHandler : IRequestHandler<ReviewPassCommand, string> {
    public Task<string> Handle(ReviewPassCommand request, CancellationToken cancellationToken) {
      // Implement the logic for handling the ReviewPassCommand here
      throw new NotImplementedException();
    }
  }
}
