<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="librarianPage.aspx.cs" Inherits="Librarian.librarianPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bokamoso LMS - Librarian Dashboard</title>
    <link rel="stylesheet" href="librarianPage.css" />
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 626px;
        }
        .auto-style3 {
            text-align: center;
        }
        .auto-style4 {
            width: 626px;
            text-align: center;
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
                </nav>

                  <div>

                <h1>&nbsp;</h1>
                      <h1>Bokamoso Library Management System</h1>
                        <h3>&nbsp;</h3>
                      <h3>Search Books</h3>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="input-field" placeholder="Search by title, author, or ISBN..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged" />

                  </div>

            </header>
            <main>
                <section class="manage-section">
                    <h2>Librarian Dashboard</h2>
                        <asp:DropDownList ID="ddlSort" runat="server" CssClass="sort-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
                            <asp:ListItem Value="BookTitle ASC" Text="Sort by Title (A-Z)" />
                            <asp:ListItem Value="BookTitle DESC" Text="Sort by Title (Z-A)" />
                            <asp:ListItem Value="BookAuthor ASC" Text="Sort by Author (A-Z)" />
                            <asp:ListItem Value="BookAuthor DESC" Text="Sort by Author (Z-A)" />
                            <asp:ListItem Value="BookGenre ASC">Sort by Genre (A-Z)</asp:ListItem>
                            <asp:ListItem Value="BookGenre DESC">Sort by Genre (Z-A)</asp:ListItem>
                        </asp:DropDownList>
                        

                        <section class="search-section">

    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <asp:HiddenField ID="hdnSearchTriggered" runat="server" />

            
            <br />
            <asp:GridView ID="gvBooks" runat="server" CssClass="books-table" CellPadding="4" ForeColor="#333333" GridLines="None" AllowSorting="True">
                <Columns>
                    <asp:TemplateField HeaderText="Select">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelect" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                    
            </Columns>
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
                __doPostBack('<%= txtSearch.UniqueID %>', '');
            }, 400);
        }

        function setupSearchDebounce() {
            const searchBox = document.getElementById('<%= txtSearch.ClientID %>');
            if (!searchBox) return;

            searchBox.addEventListener('keyup', function () {
                triggerSearchDebounced();
            });
        }

        function restoreSearchFocusIfNeeded() {
            const searchBox = document.getElementById('<%= txtSearch.ClientID %>');
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


    <br />
    
</section>


                    <p>&nbsp;</p>
                    <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
                    
                    <div class="form-group">
                        <h3>Add New Book</h3>
                        <asp:TextBox ID="txtISBN" runat="server" CssClass="input-field" placeholder="ISBN" Width="532px" />
                        <br />
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="input-field" placeholder="Book Title" Width="530px" />
                        <br />
                        <asp:TextBox ID="txtAuthor" runat="server" CssClass="input-field" placeholder="Author" Width="531px" />
                        
                        <br />
                        <asp:DropDownList ID="dropdownGenre" runat="server" Height="49px" Width="546px">
                            <asp:ListItem>Please Select your prefered genre</asp:ListItem>
                            <asp:ListItem>Academic and Reference</asp:ListItem>
                            <asp:ListItem>Business and Economics</asp:ListItem>
                            <asp:ListItem>Arts and Humanities</asp:ListItem>
                            <asp:ListItem>Social Sciences</asp:ListItem>
                            <asp:ListItem>Fiction and General Reading </asp:ListItem>
                            <asp:ListItem>Science and Technology</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <br />
                        
                        <asp:TextBox ID="txtYear" runat="server" CssClass="input-field" placeholder="Year" Width="518px"/>
                        <br />
                        <asp:Textbox ID="txtEdition" runat="server" CssClass="input-field" placeholder="Edition" Width="519px"/>
                        &nbsp;&nbsp;<br />
                        <asp:Textbox ID="txtAvailable" runat="server" CssClass="input-field" placeholder="Available copies..." Width="519px"/>
                        <br />
                        &nbsp;
                        <br />
                        <asp:Button ID="btnAddBook" runat="server" Text="Add Book" CssClass="btn-action" OnClick="btnAddBook_Click" Width="340px" />
                        <br />
                        <br />
                    </div>
                    
                    <div class="form-group">
                        <h3>Remove Book</h3>
                        <asp:TextBox ID="txtEnterISBN" runat="server" placeholder="ISBN..." CssClass="input-field" Height="25px" Width="519px"></asp:TextBox>
                        &nbsp;&nbsp;&nbsp;
                        <br />
                        <br />
                        <asp:Button ID="btnRemoveBook" runat="server" Text="Remove Selected Book" CssClass="btn-delete" OnClick="btnRemoveBook_Click" OnClientClick="return confirm('Are you sure you want to remove this book?');" />
                        <br />
                        <br />
                        <br />
                    </div>
                    
                    <div class="form-group">
                        <h3>Confirm Book Status</h3>
                        <asp:TextBox ID="txtCollectionCode" runat="server" CssClass="input-field" placeholder="Enter Collection Code" Width="1099px" />
                        &nbsp;&nbsp;&nbsp;
                        <br />
                        <asp:Button ID="btnConfirmCollection" runat="server" Text="Confirm Collection" CssClass="btn-action" OnClick="btnConfirmCollection_Click" Width="319px" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnConfirmReturn" runat="server" Text="Confirm Return" CssClass="btn-action" OnClick="btnConfirmReturn_Click" Width="302px" />
                        <br />
                        <br /><br />

                        <asp:GridView ID="gvCollectionBooks" runat="server" CssClass="books-table" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="Title" HeaderText="Title" />
                                <asp:BoundField DataField="Author" HeaderText="Author" />
                                <asp:BoundField DataField="ISBN" HeaderText="ISBN" />
                            </Columns>
                        </asp:GridView>
                        <br />
                        <br />
                        <h1>Process Payment</h1>
                        <asp:TextBox ID="txtUserID" CssClass="input-field" runat="server" placeholder="Payer ID..." Width="498px"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <br />
                        <asp:TextBox ID="txtPaymentAmount" runat="server" CssClass="input-field" placeholder="Amount..." Width="498px"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Button ID="btnCalculate" runat="server" CssClass="btn-action" BackColor="Blue"  OnClick="btnCalculate_Click"  Text="Process payment" Width="319px" />
                                 


                        <br />
                        <br />
                                 
                        

                        <br />
                        <table class="auto-style1">
                            <tr>
                                <td class="auto-style4"><h3>View Todays Collections</h3>&nbsp;</td>
                                <td class="auto-style3"><h3>View Todays Returns</h3>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style2">
                                    <asp:GridView ID="gvCollect" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="606px">
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
                                </td>
                                <td>
                                    <asp:GridView ID="gvReturn" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="583px">
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
                                </td>
                            </tr>
                        </table>
                                 
                        

                    </div>
                    
                </section>
            </main>
        </div>
    </form>
      <footer>
  <p>&copy; 2025 Bokamoso Library Management System. All Rights Reserved.</p>
</footer>
</body>

</html>


