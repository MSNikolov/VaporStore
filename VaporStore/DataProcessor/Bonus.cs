namespace VaporStore.DataProcessor
{
	using System;
    using System.Linq;
    using Data;

	public static class Bonus
	{
		public static string UpdateEmail(VaporStoreDbContext context, string username, string newEmail)
		{
			if (!context.Users.Any(u => u.Username == username))
            {
                return $"User {username} not found";
            }

            if (context.Users.Any(u => u.Email == newEmail))
            {
                return $"Email {newEmail} is already taken";
            }

            else
            {
                context.Users
                    .First(u => u.Username == username)
                    .Email = newEmail;

                context.SaveChanges();

                return $"Changed {username}'s email successfully";
            }
		}
	}
}
