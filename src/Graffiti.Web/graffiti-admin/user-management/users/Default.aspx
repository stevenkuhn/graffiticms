<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_people_Default" Title="Manage Users" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">

<Z:FileBrowser ID="FileBrowser1" runat="Server" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">

   <h1><asp:Literal runat="Server" ID="PageText" Text="Manage Users" /></h1> 
    
    <Z:Breadcrumbs runat="server" SectionName="UserManagement" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success" />
    
    <div id="new_user_container" class="FormBlock abc" runat="Server">
    
    <h3>Add a new user</h3>
    
    <h2>Username: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtUserName" Text="Please enter a UserName" runat ="Server" /></h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtUserName" TabIndex="1" MaxLength="256" />  
    
    <h2>Email: 
            <asp:RequiredFieldValidator Display="Dynamic" ID="RequiredFieldValidator3" ControlToValidate="txtEmail" Text="Please enter an Email Address" runat ="Server" />
            <asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator1" ValidationExpression="^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" runat="Server" ControlToValidate="txtEmail" Text="This email address is invalid" />
    </h2>
    
    <asp:TextBox CssClass = "small" runat="Server" id="txtEmail" TabIndex="2" MaxLength="256" />  

    <h2>Password: <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtPassword" Text="Please enter a Password" runat ="Server" /></h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtPassword" TextMode="Password" TabIndex="3" MaxLength="256" />  
    
    
    <p>
        <asp:Button runat="Server" ID="CreateUser" OnClick="CreateUser_Click" CssClass="button"  Text = "Add New User" TabIndex="4" />
    </p>
    </div>
    
    <div id="user_edit_form" class="FormBlock abc" runat="Server">
    
    
    
    <h2>
        Name: <asp:Literal runat="Server" id="txtExistingUserName"  />
        <asp:PlaceHolder runat="server" ID="AdminUserLinks" Visible="false">
            <span style="margin-left: 15px;">
            ( <asp:HyperLink runat="Server" ID="PasswordLink" Text="Change Password" style="font-size: 90%; vertical-align: .5px;" />
            <asp:Literal runat="server" ID="AdminUserLinksDelim" Visible="false" Text=" | " />
            <asp:HyperLink runat="Server" ID="RenameLink" Text="Rename User" Visible="false" style="font-size: 90%; vertical-align: .5px;" /> )
            </span>
        </asp:PlaceHolder>
    </h2>
    
    <h2>Proper Name:</h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtProperName" TabIndex="1" MaxLength="256" />  
    
    <h2>Email:</h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtExistingEmail" TabIndex="1" MaxLength="256" />  
    
    <h2>Website:</h2>
    <asp:TextBox CssClass = "small" runat="Server" id="txtWebsite" TabIndex="1" MaxLength="256" /> 
    
     <h2>Avatar:</h2>
     <asp:TextBox CssClass = "small" runat="Server" id="txtAvatar" TabIndex="6" />
     <input class="inputbutton" type="button" value="Select ..." onclick="OpenFileBrowser(new Function('url', '$$(\'<%= txtAvatar.ClientID %>\').value = url;') , 'image');return false" />
  
    
    <h2 id="role_section" runat="Server" style="padding-bottom: 5px;">Role(s):<br /></h2> 

    <div id="AllRoles" runat="server" style="padding-top: 5px;">
        <span style="padding-bottom: 5px;">
            <asp:checkbox id="chkAdmin" Text="<b>Administrator</b>" runat="server" /> (admins can do anything on your site)
        </span>
        <asp:DataList ID="Roles" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" Width="500" OnItemDataBound="Roles_OnItemDataBound">
        <ItemTemplate>
            <asp:CheckBox ID="role" runat="server" />
        </ItemTemplate>
        </asp:DataList>
    </div>
    
    <h2>Bio:</h2>
    <Z:GraffitiEditor runat="server" ID="Editor" Width="600px" ToolbarSet="Simple" TabIndex="3" />
    
    <div class="submit">
        <asp:Button runat="Server" ID="EditCategory" OnClick="EditPerson_Click" CssClass="button"  Text = "Save" TabIndex="2" />
        <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" NavigateUrl="~/graffiti-admin/user-management/" runat="Server" />
    </div>
    </div>    
    
    <div runat="Server" id ="Category_List">
    
    <Z:Repeater runat="Server" ID ="User_List">
        <NoneTemplate>
            <z:StatusMessage runat="Server" Text="Sorry, there are no users yet to list. How about you add one?" Type="Warning" />
        </NoneTemplate>
        <HeaderTemplate>
            <h3>Existing users</h3>
            <ul class="listboxes">
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <div class="nonnested">
                    <div class="title"><%# Eval("ProperName") %></div>
                    <div class="commands">
                    <a href="?user=<%# Eval("Name") %>">Edit</a>
                    <% if(CanDelete()) {%>
                    <span style="padding: 0 4px 0 4px;">|</span>
                    <a href="javascript:Telligent_Modal.Open('DeleteUser.aspx?user=<%# Eval("Name") %>', 500, 300, null);">Delete</a>
                    <% }  %>
                    </div>
                </div>
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

