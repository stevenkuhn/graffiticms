<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_categories_Default" Title="Manage Categories" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

   <h1><asp:Literal ID = "PageTitle" runat="Server" Text="Your Categories" /></h1> 
    
    <Z:Breadcrumbs runat="server" SectionName="Categories" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success" />
    
    <div id="new_category_container" class="parentlistboxes" runat="Server">
    
        <h3>Add a category</h3>
        
        <table>
            <tr>
                <td>Category Name:</td>
                <td><asp:TextBox CssClass = "short" runat="Server" id="txtCategory" TabIndex="1" MaxLength="256" />&nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtCategory" Text="Please enter a Category Name" runat ="Server" /></td>
            </tr>
            <tr>
                <td>Parent Category:</td>
                <td><asp:DropDownList runat="Server" ID = "Parent_Categories" DataTextField="Name" DataValueField="Id" TabIndex="2" /></td>
            </tr>
        </table>
        
        <div class="submit">
            <asp:Button runat="Server" ID="CreateCategory" OnClick="CreateCategory_Click" CssClass="button"  Text = "Add New Category" TabIndex="3" />
        </div>
        
    </div>
        
    <div id="category_edit_form" class="FormBlock" runat="Server">
        
        <h2>Name: <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtExistingCategory" Text="Please enter a Name" runat ="Server" /></h2>
        <asp:TextBox CssClass = "small" runat="Server" id="txtExistingCategory" TabIndex="1" MaxLength="256" />  

        <h2>Link Name:</h2>
        <asp:Literal runat="Server" ID = "existingParentLinkName" /><asp:TextBox CssClass = "small" runat="Server" id="txtExistingLinkName" TabIndex="2" MaxLength="256" /> 
        
        <h2>Description: <span class="form_label">(optional)</span></h2>
        <Z:GraffitiEditor runat="server" ID="Editor" Width="600px" ToolbarSet="Simple" TabIndex="3" />
        <script type="text/javascript">
        //<!--
        function EditSortOrder()
        {
            window.location = new String(window.location).replace(/Default.aspx/i, '').replace('?', 'PostSortOrder.aspx?');
        }
        // -->
        </script>
        
        <h2>Post Sort Order:</h2>
        <asp:RadioButtonList runat="server" ID="SortOrder">
            <asp:ListItem Value="Alphabetical">By Title, Alphabetical</asp:ListItem>
            <asp:ListItem Value="Ascending">By Date, Ascending</asp:ListItem>
            <asp:ListItem Value="Descending">By Date, Descending</asp:ListItem>
            <asp:ListItem Value="Views">By Views, Descending</asp:ListItem>
            <asp:ListItem Value="Custom">Custom (<a href="#" onclick="EditSortOrder(); return false;">Set Order</a>)</asp:ListItem>
        </asp:RadioButtonList>
        
        <h2>Meta Description: <span class="form_tip">(The meta descrption for this category)</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtMetaScription" TabIndex="5" TextMode="MultiLine" Rows="3" MaxLength="255" /> 

        <h2>Meta Keywords: <span class="form_tip">(The meta keywords for this category.)</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtKeywords" TabIndex="6" MaxLength="255" />        
        
        <h2>FeedBurner Url: <span class="form_label">(See <a href="http://feedburner.com" target="_blank">FeedBurner.com</a> for more details)</span></h2>
        <asp:TextBox CssClass = "large" runat="Server" id="txtFeedBurner" TabIndex="7" /> 
        
        <div class="submit">
            <asp:Button runat="Server" ID="EditCategory" OnClick="EditCategory_Click" Text = "Save Category" TabIndex="8" />
            <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/categories/" runat="Server" TabIndex="9" />
        </div>
        
    </div>
    
    <div runat="Server" id ="Category_List">
    
        <h3>Current categories</h3>
        
        <Z:Repeater runat="Server" ID ="Categories_List">
            <HeaderTemplate>
                <ul class="listboxes">
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <div class="nonnested">
                        <div class="title"><%# Eval("Name") %></div>
                        <div class="commands"><a href="?id=<%# Eval("Id") %>">Edit</a> | <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%# Eval("Id") %>' OnCommand="lbDelete_Command" Text="Delete" CausesValidation="false" /></div>
                    </div>
                    <Z:Repeater ID="NestedCategoriesList" runat="server">
                        <ItemTemplate>
                            <div class="nested1_pic">
                                <asp:image id="TreeImage" runat="server" />
                            </div>
                            <div class="nested1">
                                <div class="title"><%# Eval("Name") %></div>
                                <div class="commands"><a href="?id=<%# Eval("Id") %>">Edit</a> | <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%# Eval("Id") %>' OnCommand="lbDelete_Command" Text="Delete" CausesValidation="false" /></div>
                            </div>
                        </ItemTemplate>
                    </Z:Repeater>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </Z:Repeater>
    
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
</asp:Content>

