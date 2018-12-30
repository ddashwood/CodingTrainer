<%
    Response.StatusCode = 404
%>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Not Found - Code Runner</title>
    <link href="/Content/bootstrap.css" rel="stylesheet" />
    <link href="/Content/site.css" rel="stylesheet" />
</head>
<body>
    <div id="main-navbar" class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="/">Coding Trainer</a>
            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li><a href="/">Home</a></li>
                    <li><a href="/Home/Contact">Contact</a></li>
                </ul>

            </div>
        </div>
    </div>
    <div class="container body-content">
        <div class="row">
            <div class="container-fluid">
                <div class="row row-offcanvas row-offcanvas-left">
                    <h1>Not Found</h1>
                    <p>The page you are looking for cannot be found. <a href="/">Click here to return to the home page</a></p>
                </div>
            </div>
        </div>
        <hr />
        <footer>
            <p>&copy; 2018 - Coding Trainer</p>
        </footer>
    </div>
</body>
</html>
