<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FOD.Models.AirportsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Airports
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float: left; width: 80%">
        <h2>
            Airports</h2>
        <table>
            <thead>
                <tr>
                    <td>
                        Code
                    </td>
                    <td>
                        Name
                    </td>
                    <td>
                        Country
                    </td>
                </tr>
            </thead>
            <tbody>
                <% foreach (var v in Model.Airports)
                   { %>
                <tr>
                    <td>
                        <%: v.Id  %>
                    </td>
                    <td>
                        <%: v.Name  %>
                    </td>
                    <td>
                        <%: v.Country  %>
                    </td>
                </tr>
                <% } %>
            </tbody>
        </table>
    </div>

     <div style="clear: both">
    </div>
</asp:Content>
