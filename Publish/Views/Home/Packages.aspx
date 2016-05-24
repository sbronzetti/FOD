<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FOD.Models.PackageModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Voli
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Pacchetti</h2>
   
    
    <table id="tableVoli" class="tablesorter">
        <thead>
            <tr>
                <th>
                    &euro;
                </th>
                <th>
                    Da/A
                </th>
                <th>
                    Dal/Al
                </th>
                <th>
                    GG/F
                </th>
                <th>
                    Volo &euro;
                </th>
                <th>
                    Park &euro;
                </th>
                <th>
                    Carb &euro;
                </th>
                <th>
                    Autos &euro;
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var v in Model.Voli)
               { %>
            <tr>
                <td>
                    <b>
                        <%: v.PackagePrice  %></b>
                </td>
                <td>
                    <%: v.From  %>   <%: v.To  %>
                </td>
                <td>
                 <%: ((DateTime)v.Dal).ToString("dd/MM/yyyy") %>   <%: ((DateTime)v.Al).ToString("dd/MM/yyyy")%>
                </td>
                <td>
                    <%: v.Days %> / <%: v.GiorniFerie %>
                </td>
                <td>
                   <%: v.Price  %>
                </td>
               
                <td>
                   <%: v.Parcheggio  %>
                </td>
                <td>
                   <%: v.Carburante  %>
                </td>
                <td>
                   <%: v.Autostrada  %>
                </td>
                
                
            </tr>
            <% } %>
        </tbody>
    </table>

   
    <div style="clear:both"></div>
   
   
</asp:Content>
