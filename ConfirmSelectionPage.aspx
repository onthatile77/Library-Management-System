<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmSelectionPage.aspx.cs" Inherits="Librarian.ConfirmSelectionPage" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Booking Confirmation</title>
    <!-- Tailwind CSS CDN -->
    <script src="https://cdn.tailwindcss.com"></script>
    <!-- Lottie Player CDN -->
    <script src="https://unpkg.com/@lottiefiles/lottie-player@latest/dist/lottie-player.js"></script>
    <style>
        /* Custom fade-in animation */
        .fade-in {
            animation: fadeIn 1s ease-in;
        }
        @keyframes fadeIn {
            0% { opacity: 0; transform: translateY(20px); }
            100% { opacity: 1; transform: translateY(0); }
        }
        .lottie-container {
            width: 150px;
            height: 150px;
            margin: 0 auto;
        }
    </style>
</head>
<body class="bg-gray-100 flex items-center justify-center min-h-screen">
    <form id="form1" runat="server">
        <div class="bg-white p-6 rounded-lg shadow-lg max-w-md w-full fade-in">
            <!-- Lottie Animation -->
            <div class="lottie-container">
                <lottie-player 
                    src="https://assets1.lottiefiles.com/packages/lf20_jbrw3hka.json" 
                    background="transparent" 
                    speed="1" 
                    loop 
                    autoplay>
                </lottie-player>
            </div>
            <!-- Confirmation Message -->
            <h1 class="text-2xl font-bold text-center text-green-600 mb-4">Success!</h1>
            <p class="text-lg text-center text-gray-700 mb-6">
                Books successfully reserved for collection.
            </p>
            <!-- Back Button -->
            <div class="text-center">
                <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" 
                    CssClass="inline-block bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition duration-300" 
                    OnClick="btnBack_Click" />
            </div>
        </div>
    </form>
</body>
</html>