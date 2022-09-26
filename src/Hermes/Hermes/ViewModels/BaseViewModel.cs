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

		protected void Error(string msgError, Exception ex)
		{
			Log.Error(ex, msgError);
			
			Notification.Clear();
			Notification.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
			Notification.Add(msgError, Severity.Error);
		}

		protected void Success(string msgLog, string msgNotif)
		{
			Log.Information(msgLog);
			
			Notification.Clear();
			Notification.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
			Notification.Add(msgNotif, Severity.Success);
		}

		protected void Success(string msg)
		{
			Log.Information(msg);

			Notification.Clear();
			Notification.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
			Notification.Add(msg, Severity.Success);
		}


	}
}
