<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="Librarian.AdminPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bokamoso LMS - Admin Dashboard</title>
    <link rel="stylesheet" href="AdminPage.css" />
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 185px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <header>
                
                <nav>
                    <asp:HyperLink ID="lnkHome" runat="server" NavigateUrl="landingPage.aspx" Text="Home" CssClass="nav-link" />
                    <asp:HyperLink ID="lnkStudentPage" runat="server" NavigateUrl="studentPage.aspx" Text="Student Dashboard" CssClass="nav-link" />
                    <asp:HyperLink ID="lnkLibrarianPage" runat="server" NavigateUrl="librarianPage.aspx" Text="Librarian Dashboard" CssClass="nav-link" />
                </nav>
                
                <h1>&nbsp;</h1>
                <h1>Bokamoso Library Management System</h1>
                <p>&nbsp;</p>
            </header>
            <main>
                <section class="manage-section">
                    <h2>Admin Dashboard</h2>
                    <p>&nbsp;</p>
                    <asp:Label ID="lblMessage" runat="server" CssClass="message" />
                    
                    <div class="form-group">
                        <h2>Add New Librarian</h2>
                        <p>&nbsp;</p>
                        <table class="auto-style1">
                            <tr>
                                <td class="auto-style2">
                        <asp:TextBox ID="txtLibrarianFirstName" runat="server" CssClass="input-field" placeholder="First Name" Width="806px" />
                                </td>
                                <td>
                                    &nbsp;
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtLibrarianFirstName" ErrorMessage=" First Name Required!!!" Font-Bold="True" ForeColor="Red"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="auto-style2">
                        <asp:TextBox ID="txtLibrarianLastName" runat="server" CssClass="input-field" placeholder="Last Name" Width="804px" />
                                </td>
                                <td>
                                    &nbsp;
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtLibrarianLastName" ErrorMessage="Last Name Required!!" Font-Bold="True" ForeColor="Red"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="auto-style2">
                        <asp:TextBox ID="txtLibrarianEmail" runat="server" CssClass="input-field" placeholder="Email" Width="803px" />
                                    <br />
                                </td>
                                <td>
                                    &nbsp;
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtLibrarianEmail" ErrorMessage="Email Required!!" Font-Bold="True" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="auto-style2">
                        <asp:TextBox ID="txtLibrarianPassword" runat="server" TextMode="Password" CssClass="input-field" placeholder="Password" Width="802px" />
                                </td>
                                <td>
                                    &nbsp;
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtLibrarianPassword" ErrorMessage="PassWord Required!!!" Font-Bold="True" ForeColor="Red"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:RadioButton ID="radAdmin" runat="server" GroupName="radRole" Text="Admin" />
&nbsp;&nbsp;&nbsp;
                        <asp:RadioButton ID="radLib" runat="server" GroupName="radRole" Text="Librarian" />
                        <br />
                        <br />
                        <br />
                        <asp:Button ID="btnAddLibAdm" runat="server" Text="Add Librarian/Admin" CssClass="btn-action" OnClick="btnAddLibAdm_Click" />
                    </div>
                    
                    <div class="form-group">
                        <h2>Remove Librarian or User</h2>
                        <asp:TextBox ID="txtUserIdDel" runat="server" Width="1174px"></asp:TextBox>
                        <br />
                        <br />
                        <asp:GridView ID="GridView3" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="1112px">
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#0000A9" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#000065" />
                        </asp:GridView>
                        <br />
                        <br />
                        <asp:Button ID="btnRemoveUser" runat="server" Text="Remove Selected" CssClass="btn-delete" OnClick="btnRemoveUser_Click" OnClientClick="return confirm('Are you sure you want to remove this user? This cannot be undone.');" />
                    </div>
                    
                    <div class="form-group">
                        <h2>&nbsp;</h2>
                        <h2>Update Librarian or User</h2>
                        <p>&nbsp;</p>
                        <asp:GridView ID="GridView2" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="1112px">
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#0000A9" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#000065" />
                        </asp:GridView>
                        <br />
                        <br />
                        <asp:RadioButton ID="radEmail" runat="server" GroupName="Chosen" Text="Email" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:RadioButton ID="radLast" runat="server" GroupName="Chosen" Text="Last Name"  />
                        &nbsp;&nbsp;&nbsp;
                        <asp:RadioButton ID="radPass" runat="server" GroupName="Chosen" Text="Password" />
                        <br />
                        <br />
                        <asp:TextBox ID="txtUserID" runat="server" CssClass="input-field" placeholder="Enter ID to change"/>
                        <br />
                        <br />
                        <asp:TextBox ID="txtUpdate" runat="server" CssClass="input-field" placeholder="Enter new Email/Name/Password" />
                        &nbsp;
                        <br />
                        <asp:Button ID="btnUpdateUser" runat="server" BackColor="#3366FF" Height="54px" OnClick="btnUpdateUser_Click" AutoPostback="true" Text="Update Info" Width="241px" CssClass="btn-action" CausesValidation="false" />
                        <br />
                        <asp:Label ID="Label1" runat="server" ForeColor="Red" />
                        <br />
                    </div>
                    
                    <div class="form-group">
                        <h2>View All Users</h2>
                        <asp:TextBox ID="txtSearchUsers" runat="server" CssClass="input-field" placeholder="Search by name or ID..." AutoPostBack="true" OnTextChanged="txtSearchUsers_TextChanged" />
                        <asp:DropDownList ID="ddlSortUsers" runat="server" CssClass="sort-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlSortUsers_SelectedIndexChanged">
                            <asp:ListItem Value="Name ASC" Text="Sort by Name (A-Z)" />
                            <asp:ListItem Value="Name DESC" Text="Sort by Name (Z-A)" />
                            <asp:ListItem Value="ID ASC" Text="Sort by ID (A-Z)" />
                            <asp:ListItem Value="ID DESC" Text="Sort by ID (Z-A)" />
                        </asp:DropDownList>
                        

<asp:ScriptManager ID="ScriptManager1" runat="server" />
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        
        <asp:HiddenField ID="hdnSearchTriggered" runat="server" />

        
        <br />
        <asp:GridView ID="gvUsers" runat="server" CssClass="books-table" CellPadding="4" ForeColor="#333333" Width="90%">
        
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>      
        

    </ContentTemplate>
</asp:UpdatePanel>
            
            
            

    <script type="text/javascript">
        let debounceTimer;

        function triggerSearchDebounced() {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(function () {
                // ✅ Mark that search triggered the postback
                document.getElementById('<%= hdnSearchTriggered.ClientID %>').value = 'true';
                __doPostBack('<%= txtSearchUsers.UniqueID %>', '');
            }, 400);
        }

        function setupSearchDebounce() {
            const searchBox = document.getElementById('<%= txtSearchUsers.ClientID %>');
            if (!searchBox) return;

            searchBox.addEventListener('keyup', function () {
                triggerSearchDebounced();
            });
        }

        function restoreSearchFocusIfNeeded() {
            const searchBox = document.getElementById('<%= txtSearchUsers.ClientID %>');
            const hiddenField = document.getElementById('<%= hdnSearchTriggered.ClientID %>');

            if (hiddenField && hiddenField.value === 'true') {
                hiddenField.value = ''; // Reset flag
                if (searchBox) {
                    searchBox.focus();
                    const val = searchBox.value;
                    searchBox.value = '';
                    searchBox.value = val;
                }
            }
        }

        if (typeof window.searchDebounceInitialized === 'undefined') {
            window.searchDebounceInitialized = true;
            setupSearchDebounce();
        }

        window.onload = function () {
            restoreSearchFocusIfNeeded();
        };
    </script>
                    </div>
                </section>
            </main>
        </div>
    </form>

     
</body>
        <footer>
    <p>&copy; 2025 Bokamoso Library Management System. All Rights Reserved.</p>
</footer>
</html>