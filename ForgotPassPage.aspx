<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassPage.aspx.cs" Inherits="Librarian.ForgotPassPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forgot Password</title>
    <!-- Include jQuery from CDN for unobtrusive validation -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <style type="text/css">
        body {
            margin: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: #ecf0f1;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }
        .forgot-container {
            background: #fff;
            padding: 2rem;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            max-width: 500px;
            width: 100%;
            text-align: center;
        }
        .forgot-container h2 {
            color: #2c3e50;
            font-size: 2rem;
            margin-bottom: 1rem;
        }
        .form-group {
            margin-bottom: 1.5rem;
            text-align: left;
        }
        .form-group label {
            display: block;
            font-size: 1.1rem;
            color: #2c3e50;
            margin-bottom: 0.5rem;
        }
        .form-group input {
            width: 100%;
            padding: 0.7rem;
            border: 1px solid #ccc;
            border-radius: 5px;
            font-size: 1rem;
            box-sizing: border-box;
            transition: border-color 0.3s;
        }
        .form-group input:focus {
            border-color: #f39c12;
            outline: none;
        }
        .form-group .error {
            color: #e74c3c;
            font-size: 0.9rem;
            margin-top: 0.3rem;
            display: block;
        }
        .btn-submit {
            background: #2980b9;
            color: #fff;
            padding: 0.7rem 1.5rem;
            border: none;
            border-radius: 5px;
            font-size: 1rem;
            cursor: pointer;
            transition: background 0.3s;
            width: 100%;
            margin-top: 1rem;
        }
        .btn-submit:hover {
            background: #d35400;
        }
        .login-link {
            color: #3498db;
            text-decoration: none;
            font-size: 0.9rem;
            display: inline-block;
            margin-top: 1rem;
        }
        .login-link:hover {
            color: #2980b9;
        }
        .message {
            font-size: 1rem;
            color: #2c3e50;
            margin-top: 1rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="forgot-container">
            <h2>Forgot Password</h2>
            <p>Enter your email address to receive a password reset link.</p>
            <div class="form-group">
                <asp:Label ID="lblEmail" runat="server" Text="Email:"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ErrorMessage="Required!" CssClass="error" 
                    ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
            </div>
            <asp:Label ID="lblMessage" runat="server" CssClass="message"></asp:Label>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
                OnClick="btnSubmit_Click" CssClass="btn-submit" />
            <div>
                <asp:HyperLink ID="HyperLink1" runat="server" CssClass="login-link" 
                    NavigateUrl="~/LoginPage.aspx">Back to Login</asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>