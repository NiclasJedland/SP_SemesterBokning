<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Semester.ascx.cs" Inherits="HOMEASSIGNMENT.Semester.Semester" %>

<asp:Label ID="lblHolidayText" runat="server" Text="Aktuella Ansökningar för: "></asp:Label>
<asp:Label ID="lblHoliday" runat="server" Text=" "></asp:Label><br />

<asp:ListBox ID="lstHoliday" runat="server" Height="200px" Width="1200px" AutoPostBack="True" OnSelectedIndexChanged="lstHoliday_SelectedIndexChanged"></asp:ListBox><br />
<br />
<asp:Button ID="cmdCreateHolidayPetition" runat="server" Text="Skapa Ansökan" OnClick="cmdCreateHolidayPetition_Click" />
<asp:Button ID="cmdDeleteHolidayPetition" runat="server" Text="Ta bort Ansökan" OnClick="cmdDeleteHolidayPetition_Click" />
<asp:Button ID="cmdChangeHolidayPetition" runat="server" Text="Ändra Ansökan" OnClick="cmdChangeHolidayPetition_Click" />
<asp:Button ID="cmdAdminChangeStatus" runat="server" Text="Admin" OnClick="cmdAdminChangeStatus_Click" Visible="false" /><br />
<asp:Label ID="lblWarning" runat="server" Text=" "></asp:Label>

<br />
<br />
<div id="hiddenDiv" runat="server" visible="false">

	<asp:Label ID="lblNameText" runat="server" Text="Namn: "></asp:Label>
	<asp:Label ID="lblName" runat="server" Text=" "></asp:Label><br />
	<br />

	<asp:Label ID="lblStartText" runat="server" Text="Startdatum: "></asp:Label>
	<SharePoint:DateTimeControl ID="dtStart" runat="server" DateOnly="True" HoursMode24="True" />
	<br />

	<asp:Label ID="lblEndText" runat="server" Text="Slutdatum: "></asp:Label>
	<SharePoint:DateTimeControl ID="dtEnd" runat="server" DateOnly="True" HoursMode24="True" />
	<br />

	<asp:Label ID="lblStatusText" runat="server" Text="Status: "></asp:Label>
	<asp:Label ID="lblStatus" runat="server" Text="Skapad"></asp:Label><br />

	<asp:Label ID="lblPercentText" runat="server" Text="Omfattning: "></asp:Label>
	<asp:TextBox ID="txtPercent" runat="server">0</asp:TextBox>
	<asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPercent" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator><br />

	<asp:Label ID="lblAdminText" runat="server" Text="Ansvarig: "></asp:Label>
	<asp:DropDownList ID="ddlAdmin" runat="server"></asp:DropDownList><br />
	<br />
	<asp:Button ID="cmdSendHolidayPetition" runat="server" Text="Skicka Ansökan" OnClick="cmdSendHolidayPetition_Click" />
	<asp:Button ID="cmdUpdateHolidayPetition" runat="server" Text="Uppdatera Ansökan" OnClick="cmdUpdateHolidayPetition_Click" /><br />

</div>

<div id="adminDiv" runat="server" visible="false">
	<asp:Label ID="lblNameAdminText" runat="server" Text="Namn: "></asp:Label>
	<asp:Label ID="lblNameAdmin" runat="server" Text=" "></asp:Label><br />
	<br />

	<asp:Label ID="lblStartAdminText" runat="server" Text="Startdatum: "></asp:Label>
	<asp:Label ID="lblStartAdmin" runat="server" Text=" "></asp:Label>
	<br />

	<asp:Label ID="lblEndAdminText" runat="server" Text="Slutdatum: "></asp:Label>
	<asp:Label ID="lblEndAdmin" runat="server" Text=" "></asp:Label>
	<br />

	<asp:Label ID="lblStatusAdminText" runat="server" Text="Status: "></asp:Label>
	<asp:DropDownList ID="ddlAdminStatus" runat="server"></asp:DropDownList><br />

	<asp:Label ID="lblPercentAdminText" runat="server" Text="Omfattning: "></asp:Label>
	<asp:Label ID="lblPercentAdmin" runat="server" Text=" "></asp:Label><br />

	<asp:Label ID="lblAdminAdminText" runat="server" Text="Ansvarig: "></asp:Label>
	<asp:Label ID="lblAdminAdmin" runat="server" Text=" "></asp:Label><br />
	<br />
	<asp:Button ID="cmdUpdateStatus" runat="server" Text="Uppdatera Status" OnClick="cmdUpdateStatus_Click" /><br />

</div>
