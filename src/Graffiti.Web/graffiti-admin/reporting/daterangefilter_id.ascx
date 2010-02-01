<%@ Control Language="C#" %>

<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td><a href="?range=today&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">today</a></td>
        <td><a href="?range=yesterday&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">yesterday</a></td>
        <td><a href="?range=lastsevendays&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">last 7 days</a></td>
        <td><a href="?range=lastthirtydays&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">last 30 days</a></td>
        <td><a href="?range=currentmonth&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">this month</a></td>
        <td><a href="?range=lastmonth&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">last month</a></td>
        <td><a href="?range=forever&id=<%=Request.QueryString["id"] %>" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">forever</a></td>
        <td><a href="javascript: Expand()" class="CommonTextButton" style="padding: 1px; text-align: center; width: 75px">custom</a></td>
    </tr>
</table>
