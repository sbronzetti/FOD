<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FOD.Models.VoliModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Voli
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float: left; width: 80%">
        <h2>
            Best Fly</h2>
        <div style="width: 500px; height: 140px;">
            <img style="float: left;" src="../../Content/1410362782_plane.png" />
            <div style="float: left; padding: 5px; width: 200px;">
                <h3 style="color: #5c87b2; line-height: 10px;">
                    <%: Model.BestVolo.From %></h3>
                <h3 style="color: #5c87b2; line-height: 10px;">
                    <%: Model.BestVolo.To %></h3>
            </div>
            <div style="float: right; padding: 5px; line-height: 35px;">
                <h3 style="color: #5c87b2; line-height: 10px;">
                    <%: ((DateTime)Model.BestVolo.Dal).ToString("dd-MM-yyyy")%></h3>
                <h3 style="color: #5c87b2; line-height: 10px;">
                    <%: ((DateTime)Model.BestVolo.Al).ToString("dd-MM-yyyy")%>
                </h3>
            </div>
            <div style="float: left; padding: 0px; font-size: 20px;">
                <h2 style="color: #5c87b2; line-height: 30px; font-size: 30px; padding: 0px; margin: 0px;
                    text-align: right; width: 370px;">
                    &euro;
                    <%: Model.BestVolo.Price %></h2>
            </div>
        </div>
        <div style="clear: both">
        </div>
        <h2>
            Migliori Voli per destinazione</h2>
        <%--<input class="search" type="search" data-column="all" placeholder="Search entire table"/>--%>
        <table id="tableVoli" class="tablesorter">
            <thead>
                <tr>
                    <th>
                        &euro;
                    </th>
                    <th>
                        Da
                    </th>
                    <th>
                        A
                    </th>
                    <th>
                        Dal
                    </th>
                    <th>
                        Al
                    </th>
                    <th>
                        WE
                    </th>
                    <th>
                        GG
                    </th>
                    <th>
                        Trend
                    </th>
                    <th>
                        Vettore
                    </th>
                </tr>
            </thead>
            <tbody>
                <% foreach (var v in Model.Voli)
                   { %>
                <tr>
                    <td>
                        <b>
                            <%: v.Price  %></b>
                    </td>
                    <td>
                        <%: v.From  %>
                    </td>
                    <td>
                        <%: v.To  %>
                    </td>
                    <td>
                        <%: ((DateTime)v.Dal).ToString("dd/MM/yyyy") %>
                    </td>
                    <td>
                        <%: ((DateTime)v.Al).ToString("dd/MM/yyyy")%>
                    </td>
                    <% if ((Convert.ToBoolean(v.Weekend)))
                       { %>
                    <td>
                        <img width="20px" src='../../Content/1410376243_button-check_basic_blue.png' />
                    </td>
                    <% }
                       else
                       { %>
                    <td>
                    </td>
                    <% } %>
                    <td>
                        <%: v.Days.ToString()%>
                    </td>
                    <% if (v.IsPriceChanged != null && (bool)v.IsPriceChanged)
                       { %>
                    <td>
                        <% if (v.OldPrice < v.Price)
                           { %>
                        <img width="20px" src="../../Content/1410375667_arrow-left_basic_red.png" />
                        <% }
                           else
                           { %>
                        <img width="20px" src="../../Content/1410375623_arrow-up_basic_green.png" />
                        <% } %>
                    </td>
                    <% }
                       else
                       { %>
                    <td>
                    </td>
                    <% } %>
                    <td>
                        <%: v.Vettore %>
                    </td>
                </tr>
                <% } %>
            </tbody>
        </table>
    </div>
    <div style="float: right; width: 20%">
        <iframe frameborder="0" src="/Home/Processa" width="200px" height="100px"></iframe>
        <h2 style="margin-top: 105px;">
            Destinazioni trovate</h2>
        <ul style="font-size: 14px;">
            <% foreach (var d in Model.Voli.OrderBy(x=>x.To).Select(x => x.To).Distinct())
               {%>
            <li><a href="/Home/Voli/<%: d %>">
                <%: d %></a></li>
            <% } %></ul>
    </div>
    <div style="clear: both">
    </div>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#tableVoli").tablesorter({
                //widgets: ["filter"],
                //widgetOptions: {
                //    filter_external: '.search'
                //}
            });
        });
    </script>
</asp:Content>
