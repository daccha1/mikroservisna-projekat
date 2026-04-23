using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService
{
	public class Controller
	{
		private Controller instance;

		public Controller Instance
		{
			get
			{
				if (instance == null) instance = new Controller();
				return instance;
			}	
		}
		public Controller() { }

	}
}
