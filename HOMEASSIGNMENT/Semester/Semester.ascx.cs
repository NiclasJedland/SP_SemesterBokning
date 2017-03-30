using System;
using System.Linq;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Collections;
using System.Collections.Generic;

namespace HOMEASSIGNMENT.Semester
{
	[ToolboxItemAttribute(false)]
	public partial class Semester : WebPart
	{
		// Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
		// using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
		// for production. Because the SecurityPermission attribute bypasses the security check for callers of
		// your constructor, it's not recommended for production purposes.
		// [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
		public Semester()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			InitializeControl();
			OnLoad();
			lblWarning.ForeColor = System.Drawing.Color.Red;

		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		private void OnLoad()
		{
			ClearAll();

			var site = SPContext.Current.Site;
			var web = SPContext.Current.Web;

			var group = web.Groups;
			var currentUser = web.CurrentUser;

			lblHoliday.Text = currentUser.Name;

			var semester = web.Lists["SemesterLista"];

			var adminFor = new List<ListItem>();
			foreach (SPListItem item in semester.Items)
			{
				var itemDate = DateTime.Parse(item["SlutDatum"].ToString());

				var adminId = new SPFieldLookupValue(item["Ansvarig"].ToString());
				var authorID = new SPFieldLookupValue(item["Skapad av"].ToString()).LookupId;

				if (authorID == currentUser.ID && itemDate > DateTime.Now)
				{
					var listitem = new ListItem()
					{
						Text = "Namn: " + item["Förnamn"].ToString().ToUpper() + " " + item["Efternamn"].ToString().ToUpper()
							+ ", Start: " + DateTime.Parse(item["StartDatum"].ToString()).ToString("yyyy-MM-dd")
							+ ", Slut: " + DateTime.Parse(item["SlutDatum"].ToString()).ToString("yyyy-MM-dd")
							+ ", Status: " + item["Status"].ToString()
							+ ", Omfattning: " + item["Omfattning"].ToString() + "%"
							+ ", Ansvarig: " + adminId.LookupValue.ToUpper(),
						Value = item.ID.ToString()
					};
					lstHoliday.Items.Add(listitem);
				}
				else if (adminId.LookupId == currentUser.ID && itemDate > DateTime.Now)
				{
					var listitem = new ListItem()
					{
						Text = "Namn: " + item["Förnamn"].ToString().ToUpper() + " " + item["Efternamn"].ToString().ToUpper()
							+ ", Start: " + DateTime.Parse(item["StartDatum"].ToString()).ToString("yyyy-MM-dd")
							+ ", Slut: " + DateTime.Parse(item["SlutDatum"].ToString()).ToString("yyyy-MM-dd")
							+ ", Status: " + item["Status"].ToString()
							+ ", Omfattning: " + item["Omfattning"].ToString() + "%"
							+ ", Ansvarig: " + adminId.LookupValue.ToUpper(),
						Value = item.ID.ToString()
					};
					adminFor.Add(listitem);
				}
			}

			if (web.SiteGroups["SemesterAdmin"].ContainsCurrentUser)
			{
				lstHoliday.Items.Add(new ListItem() { Text = "ADMIN", Value = "NULL" });

				foreach (var item in adminFor)
				{
					lstHoliday.Items.Add(item);
				}
			}

			var status = (SPFieldChoice)semester.Fields["Status"];

			foreach (var item in status.Choices)
			{
				//if (item != "Skapad")
				//{
				var stat = new ListItem()
				{
					Text = item,
					Value = item
				};
				ddlAdminStatus.Items.Add(item);
				//}
			}

			foreach (SPGroup grp in web.Groups)
			{
				if (grp.Name == "SemesterAdmin")
				{
					foreach (SPUser user in grp.Users)
					{
						var admin = new ListItem()
						{
							Value = user.ID.ToString(),
							Text = user.Name
						};
						ddlAdmin.Items.Add(admin);
					}
				}
			}
		}

		protected void cmdCreateHolidayPetition_Click(object sender, EventArgs e)
		{
			cmdUpdateHolidayPetition.Visible = false;
			cmdSendHolidayPetition.Visible = true;

			lstHoliday.ClearSelection();

			txtPercent.Text = "100";
			dtEnd.ClearSelection();
			dtStart.ClearSelection();

			adminDiv.Visible = false;
			hiddenDiv.Visible = true;

			var web = SPContext.Current.Web;
			var currentUser = web.CurrentUser;

			lblName.Text = currentUser.Name;
		}
		
		protected void cmdSendHolidayPetition_Click(object sender, EventArgs e)
		{
			if (dtStart.SelectedDate != null && ddlAdmin.SelectedItem != null && dtEnd.SelectedDate != null && ddlAdmin.SelectedItem != null && double.TryParse(txtPercent.Text, out double percent))
			{
				var list = SPContext.Current.Web.Lists["SemesterLista"];
				var currentUser = SPContext.Current.Web.CurrentUser;

				var currentUserName = currentUser.Name.Split(' ').ToList();

				var newItem = list.Items.Add();

				newItem["Förnamn"] = currentUserName.Count > 0 ? currentUserName[0] : "NULL";
				newItem["Efternamn"] = currentUserName.Count > 1 ? currentUserName[1] : "NULL";
				newItem["StartDatum"] = dtStart.SelectedDate;
				newItem["SlutDatum"] = dtEnd.SelectedDate;
				newItem["Status"] = "Skapad";
				newItem["Omfattning"] = percent;
				newItem["Ansvarig"] = ddlAdmin.SelectedValue;
				newItem["Skapad av"] = currentUser;

				if (!DateOverlaps(dtStart.SelectedDate, dtEnd.SelectedDate))
				{
					newItem.Update();
					OnLoad();
				}
				else
				{
					lblWarning.Text = "Date Overlaps";
				}
			}
			else
			{
				lblWarning.Text = "Must have a valid date, and between 1-100%";
			}
		}

		protected void cmdDeleteHolidayPetition_Click(object sender, EventArgs e)
		{
			if (lstHoliday.SelectedItem != null)
			{
				var delete = SPContext.Current.Web.Lists["SemesterLista"].GetItemById(int.Parse(lstHoliday.SelectedValue));
				if (delete["Status"].ToString() != "Beviljad" && delete["Status"].ToString() != "Avslagen")
				{
					delete.Delete();
					OnLoad();
				}
				else
				{
					lblWarning.Text = "Cannot delete with that status!";
				}
			}
		}

		protected void cmdChangeHolidayPetition_Click(object sender, EventArgs e)
		{
			if (lstHoliday.SelectedItem != null)
			{
				var selected = SPContext.Current.Web.Lists["SemesterLista"].GetItemById(int.Parse(lstHoliday.SelectedValue));

				if (selected["Status"].ToString() != "Beviljad" && selected["Status"].ToString() != "Avslagen" && DateTime.Parse(selected["StartDatum"].ToString()) > DateTime.Now)
				{
					cmdSendHolidayPetition.Visible = false;
					cmdUpdateHolidayPetition.Visible = true;

					hiddenDiv.Visible = true;
					adminDiv.Visible = false;

					var list = SPContext.Current.Web.Lists["SemesterLista"];

					// Get selecteditem status value
					var selectedItem = list.GetItemById(int.Parse(lstHoliday.SelectedValue));
					ddlAdminStatus.Text = selectedItem["Status"].ToString();

					// Get selecteditem start/end dates
					dtStart.SelectedDate = DateTime.Parse(selectedItem["StartDatum"].ToString());
					dtEnd.SelectedDate = DateTime.Parse(selectedItem["SlutDatum"].ToString());

					// Get selecteditem percentage and admin
					txtPercent.Text = (double.Parse(selectedItem["Omfattning"].ToString())).ToString();

					//TODO: SelectedAdmin
					//ddlAdmin.SelectedValue = selectedItem["Ansvarig"].ToString();
					//ddlAdmin.Items.FindByValue(selectedItem["Ansvarig"].ToString()).Selected = true;
				}
				else
				{
					lblWarning.Text = "Must have a valid date, and between 1-100%";
				}
			}
		}

		protected void cmdAdminChangeStatus_Click(object sender, EventArgs e)
		{
			if (lstHoliday.SelectedItem != null)
			{
				hiddenDiv.Visible = false;
				adminDiv.Visible = true;

				var web = SPContext.Current.Web;
				var list = web.Lists["SemesterLista"];

				// Get selecteditem status value
				var selectedItem = list.GetItemById(int.Parse(lstHoliday.SelectedValue));
				ddlAdminStatus.SelectedValue = selectedItem["Status"].ToString();

				// Get selecteditem start/end dates
				lblStartAdmin.Text = DateTime.Parse(selectedItem["StartDatum"].ToString()).ToString("yyyy-MM-dd");
				lblEndAdmin.Text = DateTime.Parse(selectedItem["SlutDatum"].ToString()).ToString("yyyy-MM-dd");

				// Get selecteditem percentage and admin
				lblPercentAdmin.Text = (double.Parse(selectedItem["Omfattning"].ToString())).ToString() + "%";
				lblAdminAdmin.Text = selectedItem["Ansvarig"].ToString();

				lblNameAdmin.Text = new SPFieldLookupValue(selectedItem["Skapad av"].ToString()).LookupValue;
			}
		}

		protected void cmdUpdateHolidayPetition_Click(object sender, EventArgs e)
		{
			if (lstHoliday.SelectedItem != null && dtStart.SelectedDate != null && dtEnd.SelectedDate != null && ddlAdmin.SelectedItem != null && double.TryParse(txtPercent.Text, out double percent))
			{
				var update = SPContext.Current.Web.Lists["SemesterLista"].GetItemById(int.Parse(lstHoliday.SelectedValue));
				update["StartDatum"] = dtStart.SelectedDate;
				update["SlutDatum"] = dtEnd.SelectedDate;
				update["Status"] = "Skapad";
				update["Omfattning"] = percent;
				update["Ansvarig"] = ddlAdmin.SelectedValue;

				if (!DateOverlaps(update.ID, dtStart.SelectedDate, dtEnd.SelectedDate))
				{
					update.Update();
					OnLoad();
				}
				else
				{
					lblWarning.Text = "Date Overlaps";
				}
			}
			else
			{
				lblWarning.Text = "Must have a valid date, and between 1-100%";
			}
		}

		protected void cmdUpdateStatus_Click(object sender, EventArgs e)
		{
			if (lstHoliday.SelectedItem != null && ddlAdminStatus.SelectedItem != null)
			{
				var update = SPContext.Current.Web.Lists["SemesterLista"].GetItemById(int.Parse(lstHoliday.SelectedValue));
				update["Status"] = ddlAdminStatus.SelectedValue;
				update.Update();
				OnLoad();
			}
		}

		protected void lstHoliday_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstHoliday.SelectedValue == "NULL")
			{
				cmdDeleteHolidayPetition.Visible = false;
				cmdChangeHolidayPetition.Visible = false;
				cmdAdminChangeStatus.Visible = false;
			}
			else if (lstHoliday.SelectedItem != null)
			{
				var web = SPContext.Current.Web;
				var currentUser = web.CurrentUser;

				var currentID = currentUser.ID;
				var selectedItem = web.Lists["SemesterLista"].GetItemById(int.Parse(lstHoliday.SelectedValue));
				var createdBy = new SPFieldLookupValue(selectedItem["Skapad av"].ToString()).LookupId;

				if (currentUser.ID == createdBy && selectedItem["Status"].ToString() != "Beviljad" && selectedItem["Status"].ToString() != "Avslagen")
				{
					cmdDeleteHolidayPetition.Visible = true;
					cmdChangeHolidayPetition.Visible = true;
				}
				else
				{
					cmdDeleteHolidayPetition.Visible = false;
					cmdChangeHolidayPetition.Visible = false;
				}

				var admin = new SPFieldLookupValue(selectedItem["Ansvarig"].ToString()).LookupId;

				if (web.SiteGroups["SemesterAdmin"].ContainsCurrentUser && admin == currentUser.ID)
					cmdAdminChangeStatus.Visible = true;
				else
					cmdAdminChangeStatus.Visible = false;
			}
		}

		private void ClearAll()
		{
			lblWarning.Text = " ";
			txtPercent.Text = "100";

			adminDiv.Visible = false;
			hiddenDiv.Visible = false;

			dtStart.ClearSelection();
			dtEnd.ClearSelection();

			dtStart.MinDate = DateTime.Today.AddDays(1);
			dtStart.MaxDate = DateTime.Today.AddDays(365);
			dtEnd.MinDate = DateTime.Today.AddDays(1);
			dtEnd.MaxDate = DateTime.Today.AddDays(365);

			lstHoliday.Items.Clear();
			ddlAdmin.Items.Clear();
			ddlAdminStatus.Items.Clear();

			cmdAdminChangeStatus.Visible = false;
			cmdChangeHolidayPetition.Visible = true;
			cmdDeleteHolidayPetition.Visible = true;
			cmdCreateHolidayPetition.Visible = true;

			cmdUpdateHolidayPetition.Visible = true;
			cmdSendHolidayPetition.Visible = true;
			cmdUpdateStatus.Visible = true;
		}

		private bool DateOverlaps(DateTime start, DateTime end)
		{
			if (start > end)
				return true;

			var web = SPContext.Current.Web;
			var semester = web.Lists["SemesterLista"];

			foreach (SPListItem item in semester.Items)
			{
				var createdById = new SPFieldLookupValue(item["Skapad av"].ToString()).LookupId;

				if (createdById == web.CurrentUser.ID)
				{
					var startDate = DateTime.Parse(item["StartDatum"].ToString());
					var endDate = DateTime.Parse(item["SlutDatum"].ToString());

					if (start <= endDate && startDate <= end)
						return true;
				}
			}
			return false;
		}

		private bool DateOverlaps(int itemId, DateTime start, DateTime end)
		{
			if (start > end)
				return true;

			var web = SPContext.Current.Web;
			var semester = web.Lists["SemesterLista"];

			foreach (SPListItem item in semester.Items)
			{
				if (itemId == item.ID)
					continue;

				var userId = new SPFieldLookupValue(item["Skapad av"].ToString()).LookupId;

				if (userId == web.CurrentUser.ID)
				{
					var startDate = DateTime.Parse(item["StartDatum"].ToString());
					var endDate = DateTime.Parse(item["SlutDatum"].ToString());

					if (start <= endDate && startDate <= end)
						return true;
				}
			}
			return false;
		}
	}
}
