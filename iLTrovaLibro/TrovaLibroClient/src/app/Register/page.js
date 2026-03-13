import Header from '@/components/Header';
import Footer from '@/components/Footer';
import Register from './components/Register';



export default function Page() {
    const isLoggedIn = false;

    return (
        <div className="min-h-screen bg-slate-50/50 flex flex-col">
            <Header isLoggedIn={isLoggedIn} />

            {/* Ridotto pt da 32 a 20 per eliminare il vuoto iniziale */}
            <main className="flex-1 max-w-7xl mx-auto px-10 pt-20 pb-10">
                <Register />
            </main>

            <Footer />
        </div>
    );
}