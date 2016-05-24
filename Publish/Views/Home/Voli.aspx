<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FOD.Models.VoliModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Voli
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%: Model.H2  %></h2>
    <div style="padding: 10px;">
        <b>Aeroporti</b><select style="margin-left: 10px;" id="airports">
            <option value="Tutti">Tutti</option>
            <% foreach (var d in Model.TuttiAeroporti.Select(x => x.From).Distinct())
               {%>
            <option value="<%: d.ToString() %>">
                <%: d.ToString() %></option>
            <% } %>
            <% foreach (var d in Model.TuttiAeroporti.Select(x => x.To).Distinct())
               {%>
            <option value="<%: d.ToString() %>">
                <%: d.ToString() %></option>
            <% } %>
        </select>
        <input id="btnSearch" type="button" value="Cerca" style="line-height: 15px;" />
        <b>Dal</b>
        <input type="text" id="datepicker"/>
        <input id="btnDate" type="button" value="Cerca" style="line-height: 15px;" />


       <%-- <div style="padding: 10px;">
            <b>Dal</b>
            <input id="partenza" type="text" />
        </div>
        <div style="padding: 10px;">
            <b>Al</b>
            <input id="ritorno" type="text" />
        </div>
        <input id="btnDateSearch" type="button" value="Cerca" style="line-height: 15px;" />--%>


        
    </div>
    <div style="float:left; width:70%;">
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
            <% foreach (var v in Model.TuttiVoli)
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
    <div style = "float:right; width:30%;">
    <%: Model.Wiki %>
    </div>
    <div style="clear:both"></div>
   
    <script type="text/javascript">
        $(document).ready(function () {

            $("#tableVoli").tablesorter({ dateFormat: 'dd/MM/yyyy' });
//            $("#datepicker").datepicker();


            $("#btnSearch").click(function () {
                var val = $("#airports option:selected").val();
                if (val == "Tutti") {
                    window.location.replace("/Home/Voli/");
                }
                else {
                    window.location.replace("/Home/Voli/" + val);
                }
            });
            $("#btnDate").click(function () {
                var val = $("#datepicker").val();
                var val2 = $("#airports option:selected").val();
                    window.location.replace("/Home/Voli/" + val2 + "_"+val);
                
            });



        });
    </script>
</asp:Content>
