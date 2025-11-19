﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SettingsPage.aspx.cs" Inherits="Librarian.SettingsPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <titleBokamoso LMS - Manage User Details</title>
    <link rel="stylesheet" href="SettingsPage.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <header>
                
                <nav>
                    <asp:HyperLink ID="lnkHome" runat="server" NavigateUrl="landingPage.aspx" Text="Home" CssClass="nav-link" />
                    <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="studentPage.aspx" Text="Back to Selection" CssClass="nav-link" />
                </nav>
                <br />
                
                <h1>Bokamoso Library Management System</h1>
            </header>
            <main>
                <section class="manage-section">
                    <h2>Manage User Details</h2>
                    <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />
                    
                    <div class="form-group">
                    </div>
                    
                    <div class="form-group">
                        <asp:Button ID="btnUpdatePassword" runat="server" Text="Update Password" CssClass="btn-action" OnClick="btnUpdatePassword_Click" Width="285px" />
                    </div>
                    
                    <div class="form-group">
                        <h3>Delete Account</h3>
                        <asp:Button ID="btnDeleteAccount" runat="server" Text="Delete My Account" CssClass="btn-delete" OnClick="btnDeleteAccount_Click" OnClientClick="return confirm('Are you sure you want to delete your account? This cannot be undone.');" />
                    </div>
                    
                    <div class="form-group">
                        <h3Outstanding Fees</h3>
                        <p>
                            <asp:Label ID="lblFees" runat="server"></asp:Label>
                        </p>
                        <p&nbsp;</p>
                        <p><h2>View borrowed bookss</h2>
                        <asp:GridView ID="gvBorrowed" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None">
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
                        </p>
                        <p>&nbsp;</p>
                        <h2>Cancel Open Booking</h2>
                        <p>
                            <asp:TextBox ID="txtCodeCancel" CssClass="input-field" runat="server" placeholder="Enter QR Code for Booking to cancel..."></asp:TextBox>
&nbsp;</p>
                        <p>
                            <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel Booking" Width="287px" BackColor="#FF3300" Height="51px" />
                        </p>
                    </div>
                </section>
            </main>
        </div>
    </form>
              <footer>
<p>&copy; 2025 Bokamoso Library Management System. All Rights Reserved........</p>
</footer>
</body>

    <!-- Footer -->
    

</html>