import "./globals.css";


export default function RootLayout({ children }) {
    return (
        <html lang="en">
            <body className="antialiased bg-white">
                {children}
            </body>
        </html>
    );
}
