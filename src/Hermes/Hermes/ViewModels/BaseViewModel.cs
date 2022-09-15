﻿using Hermes.DataAccess;
using MudBlazor;

namespace Hermes.ViewModels
{
	public abstract class BaseViewModel
	{
		protected IHermesContext DbContext;
		protected ISnackbar Notification;

		public BaseViewModel(IHermesContext contextHermes, ISnackbar snackbar)
		{
			DbContext = contextHermes;
			Notification = snackbar;
		}



		protected void Error(string msgLog, string msgNotif, Exception ex)
		{
			Log.Error(ex, msgLog);
			Notification.Clear();
			Notification.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
			Notification.Add(msgNotif, Severity.Error);
		}

		protected void Success(string msgLog, string msgNotif)
		{
			Log.Information(msgLog);
			Notification.Clear();
			Notification.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
			Notification.Add(msgNotif, Severity.Success);
		}

		
	}
}