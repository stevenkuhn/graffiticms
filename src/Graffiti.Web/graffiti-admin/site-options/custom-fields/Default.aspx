<%@ Page Language="C#" MasterPageFile="~/graffiti-admin/common/AdminMasterPage.master" AutoEventWireup="true" Inherits="graffiti_admin_site_options_forms_Default" Title="Graffiti - Custom Fields" Codebehind="Default.aspx.cs" %>
<%@ Register TagPrefix="Z" Assembly="Graffiti.Core" Namespace="Graffiti.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderRegion" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainRegion" Runat="Server">
<h1>Your Custom Fields</h1> 
<div id="messages_form">
    
    <Z:Breadcrumbs runat="server" SectionName="CustomFields" />
    <Z:StatusMessage runat="Server" ID = "Message"  Type="Success"/>
        
    <asp:MultiView runat="Server" ID = "FormViews" ActiveViewIndex="0">
        <asp:View ID = "NewFieldView" runat="Server">

             <div id="post_form_container" class="FormBlock abc">
             
                <h3>Add a new custom field</h3>
                
                <table>
                    <tr>
                        <td>Name:</td>
                        <td><asp:TextBox runat="Server" ID = "FieldName" CssClass="Small" TabIndex="1" /></td>
                    </tr>
                    <tr>
                        <td>Category:</td>
                        <td><asp:Label ID="lblCategory" runat="Server" /></td>
                    </tr>
                    <tr>
                        <td>Field Type:</td>
                        <td>
                            <asp:DropDownList runat="Server" ID="TypesOfField" TabIndex="3">
                                <asp:ListItem Text="Single Line" Value="TextBox" />
                                <asp:ListItem Text="Multiple Lines" Value="TextArea" />
                                <asp:ListItem Text="List" Value="List" />
                                <asp:ListItem Text="CheckBox" Value="CheckBox" />
                                <asp:ListItem Text="File Selector" Value="File" />
                                <asp:ListItem Text="DateTime" Value="DateTime" />
                             </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                 
             </div>
             
            <div class="submit">
                <div id="buttons">
                    <asp:Button ID="Publish_Button" runat="Server" Style="font-weight:bold" Text = "Add New Field"  OnClick="NewFieldBTN_Click" TabIndex="4" />
                </div>
            </div>

            <div id="CustomFieldList" runat="server" class="FormBlock">
            
                <h3>Existing custom fields</h3>
                
                <Z:Repeater runat="Server" ID = "ExistingFields" ShowHeaderFooterOnNone="False" >
                    <NoneTemplate>
                        <z:StatusMessage runat="Server" Text="No custom fields were found. Why not create one?" Type="Warning" />
                    </NoneTemplate>
                    <HeaderTemplate>
                        <ul class="listboxes">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <div class="nonnested">
                                <div class="title"><%# Eval("Name") %></div>
                                <div class="commands"><a href="?id=<%# Eval("id") %>&category=<%= Request.QueryString["category"] ?? "-1" %>">Edit</a>  | <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%# Eval("Id") %>' OnCommand="lbDelete_Command" Text="Delete" CausesValidation="false" /></div>
                            </div>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </Z:Repeater>
                
            </div>

        </asp:View>
        
        <asp:View runat="Server" ID = "EditFieldView">

            <div id="Div1" class="FormBlock abc">

                <h2>Name: <span class="form_tip">(you should not change this value after have published a post)</span></h2>
                <asp:TextBox runat="Server" ID = "ExistingName" CssClass = "large" TabIndex="1" />

                <h2>Description: <span class="form_tip">(a short description about the field. It will render just like this message)</span></h2>
                <asp:TextBox runat="Server" ID = "ExistingDescription" CssClass = "large" TabIndex="2" />

                <asp:PlaceHolder runat="Server" ID = "CheckboxRegion" Visible="false">
                <h2><asp:CheckBox runat="Server" ID = "ExistingCheckBox" TabIndex="3" Text="Default State" /><br /><span class="form_tip">(should the checkbox be selected by default?)</span></h2>    

                </asp:PlaceHolder>

                <asp:PlaceHolder runat="Server" ID = "ListRegion" Visible="False">
                    <h2>List Options:<span class="form_tip">(please add on option per line)</span></h2>
                    <asp:TextBox runat="Server" ID = "ExistingListOptions" TextMode="MultiLine" Rows="5" Columns="60" CssClass="Large" TabIndex="5" />
                </asp:PlaceHolder>


            </div>

            <div class="submit">
                <div id="Div2">
                    <asp:Button ID="Button1" runat="Server" Style="font-weight:bold" Text = "Update Field"  OnClick="UpdateFieldBTN_Click" TabIndex="81" />
                    <asp:HyperLink ID ="Cancel_Edit" Text = "(Cancel)" TabIndex="82" runat="server" />
                </div>
            </div>    
        
        </asp:View>
        
    </asp:MultiView>
    
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SideBarRegion" Runat="Server">
<div id="sidebar"><div class="gutter">
<div class="box">
    <h3>Categories</h3>
    <Z:Repeater ShowHeaderFooterOnNone="false" ID="rptCategories" runat="Server">
        <HeaderTemplate>
            <ul class="categorylist" style="margin: 5px 0 5px 0; font-size: 110%;">
            <li<%# IsSelectedCategory("-1") %> style="margin-left: -7px; margin-right: -7px;">
                <a href="?category=-1">Global (no category)</a>
            </li>
        </HeaderTemplate>
        <ItemTemplate>
            <li<%# IsSelectedCategory(Eval("ID").ToString()) %>>
                <a style="margin-left: -7px; margin-right: -7px;" href="?category=<%# Eval("Id") %>"><%# Eval("Name") %></a>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </Z:Repeater>
</div>

<div style="clear: both;"></div>
</div></div>
</asp:Content>
