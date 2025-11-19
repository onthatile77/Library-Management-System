<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentPage.aspx.cs" Inherits="Librarian.StudentPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bokamoso LMS - Student Dashboard</title>
    <link rel="stylesheet" href="StudentPage.css" />
    <style type="text/css">
        .books-table {}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <header>
               
                <nav>
                    <asp:HyperLink ID="lnkHome" runat="server" NavigateUrl="~/LandingPage.aspx" Text="Home" CssClass="nav-link" />
                    <asp:HyperLink ID="lnkSettings" runat="server" NavigateUrl="~/SettingsPage.aspx" Text="Settings" CssClass="nav-link" />
                     
                </nav>
                <h1>Bokamoso Library Management System</h1>
                <p>&nbsp;</p>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input"
                    AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"
                    placeholder="Search by title, author, or ISBN..." Width="95%" />
            </header>
            <main>
                
                    <section class="search-section">
                    
                    <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
        
                            <asp:HiddenField ID="hdnSearchTriggered" runat="server" />

        
                            <br />
                            <asp:GridView ID="gvBooks" runat="server" CssClass="books-table" CellPadding="4" ForeColor="#333333" Width="90%">
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

                <asp:Button ID="btnAddToSelection" runat="server" Text="Add to Selection" CssClass="btn-add" OnClick="btnAddToSelection_Click" />
                

                        <br />
                        
                    </section>

                <section class="selected-books">
                    <h2>Selected Books</h2>
                    <asp:ListBox ID="lstSelectedBooks" runat="server" CssClass="selected-list" />
                    
                    <br />
                    <br />
                    
                    <asp:Button ID="btnRemoveSelected" runat="server" Text="Remove Selected" CssClass="btn-remove" OnClick="btnRemoveSelected_Click" />

                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

                                        
        <br />
        
        &nbsp;
                    <br />
&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    <br />
                </section>

                <section>
                  
                       
                            &nbsp;<h2><asp:Label ID="Label1" runat="server" Text="Select Date of Collection"></asp:Label></h2>
                            <br />
                            <asp:Calendar ID="calCollectDate" runat="server" AutoPostBack="True" BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="279px" Width="90%">
                                <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                <OtherMonthDayStyle ForeColor="#999999" />
                                <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                <WeekendDayStyle BackColor="#CCCCFF" />
                            </asp:Calendar>
                        &nbsp;<asp:Label ID="lblError" runat="server" ForeColor="#FF3300"></asp:Label>
                            <br />
                            <br />

                    <asp:Button ID="btnConfirmBooking" runat="server" Text="Confirm Booking" CssClass="btn-confirm" OnClick="btnConfirmSelections_Click" />


                </section>
            </main>
        </div>
    </form>
     
</body>
        <footer>
    <p>&copy; 2025 Bokamoso Library Management System. All Rights Reserved.</p>
</footer>
</html>