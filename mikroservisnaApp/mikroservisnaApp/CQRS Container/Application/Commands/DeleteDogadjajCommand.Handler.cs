using ContractsCQRS;
using Microsoft.EntityFrameworkCore;
using StrucniDogadjaj.Infrastructure.Write.EFCore.Data;

namespace mikroservisnaApp.CQRS_Container.Application.Commands
{
	public class DeleteDogadjajCommandHandler
	{
		private WriteDbContext _context;
		public DeleteDogadjajCommandHandler(WriteDbContext context)
		{
			_context = context;
		}

		internal async Task<int> Handle(DeleteDogadjajCommand cmd)
		{
			var exist = await _context.Dogadjaji.Where(d => d.Id == cmd.IdDogadjaja).FirstOrDefaultAsync();
			if(exist == null)
			{
				return 0;
			}

			_context.Remove(exist);
			await _context.SaveChangesAsync();
			return 1;
		}
	}
}
