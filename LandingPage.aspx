<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LandingPage.aspx.cs" Inherits="Librarian.LandingPage" %>

<!-- GROUP 17
PG LEGOBYE 41270118
LO MOILA 39068196
KP MABOTE 41409914
MO Mbenyane 45090955
AL SIBEKO 506224806
CS Mkhize 40757536
S Msimanga 39961370
LSM Legodi 37198742
 -->

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Library Management System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="LandingPage.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navigation -->
        <nav class="navbar">
            <div class="nav-container">
                <div class="logo">📚 Library System</div>
                <ul class="nav-links">
                    <li><a href="#">Home</a></li>
                    <li><a href="#about">About</a></li>
                    <li><a href="#contact">Contact</a></li>
                    <li><a href="LoginPage.aspx" class="btn-login">Login</a></li>
                    <li><a href="RegisterPage.aspx" class="btn-register">Register</a></li>
                </ul>
            </div>
        </nav>

        <!-- Hero Section -->
        <section class="hero">
            
            <div class="hero-content">
                <h1>Welcome to Bokamoso Library Management System</h1>
                 <br />
                <br />
                <br />
                <h4>Manage books, track borrowing, and explore digital resources all in one place.</h4>
                <a href="LoginPage.aspx" class="btn">Get Started</a>
            </div>
        </section>

        <!-- Features -->
        <section class="features">
            <div class="feature">
                <h2>📖 Explore Catalog</h2>
                <p>Search and browse through our extensive book collection with ease.</p>
            </div>
            <div class="feature">
                <h2>👨‍🏫 User Accounts</h2>
                <p>Students and staff can log in to manage their borrowing history.</p>
            </div>
            <div class="feature">
                <h2>⚡ Fast & Reliable</h2>
                <p>Simple, intuitive, and efficient system to keep your library running smoothly.</p>
            </div>
        </section>

        <!-- About Us -->
        <section id="about" class="about">
            <div class="container">
                <h2>About Us</h2>
                <p>
                    Our Library Management System is designed to simplify and modernize library operations. 
                    We aim to provide students, teachers, and administrators with an easy-to-use platform 
                    for managing resources efficiently. 
                </p>
                <p>
                    With a passion for knowledge and innovation, our mission is to bring libraries into the 
                    digital era, making them more accessible and effective for everyone.
                </p>
            </div>
        </section>

        <!-- Contact Us -->
        <section id="contact" class="contact">
            <div class="container">
                <h2>Contact Us</h2>
                <p>Have questions or need support? Get in touch with us!</p>
                <div class="social-links">
                    <a href="https://www.facebook.com/yourprofile" target="_blank" aria-label="Facebook">
                        <i class="fab fa-facebook-f"></i>
                    </a>
                    <a href="https://wa.me/yourphonenumber" target="_blank" aria-label="WhatsApp">
                        <i class="fab fa-whatsapp"></i>
                    </a>
                    <a href="https://www.instagram.com/yourprofile" target="_blank" aria-label="Instagram">
                        <i class="fab fa-instagram"></i>
                    </a>
                    <a href="https://x.com/yourprofile" target="_blank" aria-label="Twitter">
                        <i class="fab fa-x-twitter"></i>
                    </a>
                </div>
            </div>
        </section>

        <!-- Add Font Awesome for icons -->
        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">
        <style>
            .social-links {
                display: flex;
                gap: 20px;
                justify-content: center;
                margin-top: 20px;
            }
            .social-links a {
                font-size: 24px;
                color: #333;
                transition: color 0.3s ease;
            }
            .social-links a:hover {
                color: #007bff;
            }
        </style>

        <!-- Footer -->
        <footer>
            <p>&copy; 2025 Library Management System. All Rights Reserved.</p>
        </footer>
    </form>
</body>
</html>
