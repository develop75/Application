import Header from '@/components/Header';
import BookCard from '@/components/BookCard';
import Footer from '@/components/Footer';

import { PlusCircle, Library } from 'lucide-react'; // Icone utili
import Link from 'next/link';


export default function Page() {
    const isLoggedIn = false;

    // Array di 10 libri simulati
    const books = [
        { id: 1, title: "The Hunger Games", author: "Suzanne Collins", rating: 4, price: 38.50, oldPrice: 50, city: "Verona", hasShipping:true, imageUrl: "https://m.media-amazon.com/images/I/61I24wOsn8L.jpg" },
        { id: 2, title: "To Kill a Mockingbird", author: "Harper Lee", rating: 5, price: 48.00, oldPrice: null, city: "Bologna", hasShipping: false, imageUrl: "https://m.media-amazon.com/images/I/81gepf1eMqL.jpg" },
        { id: 3, title: "1984", author: "George Orwell", rating: 4, price: 45.00, oldPrice: null, city: "Torino", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/71kxa1-0mfL.jpg" },
        { id: 4, title: "The Great Gatsby", author: "F. Scott Fitzgerald", rating: 4, price: 60.00, oldPrice: null, city: "Napoli", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/71FTByfL3uL.jpg" },
        { id: 5, title: "Dune", author: "Frank Herbert", rating: 5, price: 78.90, oldPrice: null, city: "Firenze", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/41m9mOQd94L.jpg" },
        { id: 6, title: "Moby Dick", author: "Herman Melville", rating: 3, price: 32.00, oldPrice: null, city: "Venezia", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/81S8lGf5m7L.jpg" },        
        { id: 7, title: "The Little Prince", author: "Antoine de Saint-Exupéry", rating: 5, price: 55.50, oldPrice: null, city: "Roma", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/71OZY0m96sL.jpg" },
        { id: 8, title: "The Hobbit", author: "J.R.R. Tolkien", rating: 5, price: 65.00, oldPrice: null, city: "Palermo", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/712c9v-7o9L.jpg" },
        { id: 9, title: "Frankenstein", author: "Mary Shelley", rating: 4, price: 29.90, oldPrice: null, city: "Genova", hasShipping: true, imageUrl: "https://m.media-amazon.com/images/I/81xXUfRInXL.jpg" },        
    ];

    return (

        <div className="min-h-screen bg-slate-50/50">
            <Header isLoggedIn={isLoggedIn} />

            {/* pt-16: Spazio minimo indispensabile sotto l'header */}
            <main className="max-w-7xl mx-auto px-10 pt-32 pb-20">

                {/* Header Sezione: mb-6 invece di mb-12 per avvicinare i libri */}
                <div className="flex flex-col sm:flex-row sm:items-center justify-between mb-6 gap-4">
                    <div className="flex items-center gap-3">
                        {/* Titolo più grande e bold */}
                        <h2 className="text-xl font-bold text-slate-800 tracking-tight">
                            Libri disponibili
                        </h2>
                        <div className="h-4 w-[2px] bg-slate-200 hidden sm:block" />
     

                        <p className="text-slate-400 text-xs hidden md:block">Esplora la collezione</p>
                    </div>

                    {/* Azioni Utente: Compatte */}
                    {isLoggedIn && (
                        <div className="flex items-center gap-2">
                            <Link href="/my-books" className="px-3 py-1.5 text-[11px] font-bold text-slate-500 hover:text-orange-500 transition-colors border border-slate-200 rounded-lg bg-white">
                                I miei libri
                            </Link>

                            <Link href="/add-book" className="px-3 py-1.5 text-[11px] font-bold text-white bg-orange-500 hover:bg-orange-600 rounded-lg shadow-sm">
                                + Inserisci
                            </Link>
                        </div>
                    )}
                </div>

                {/* Griglia: Ridotto gap-y per compattare le righe */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-x-8 gap-y-12 justify-items-center">
                    {books.map((book) => (
                        <BookCard
                            key={book.id}
                            {...book}
                            isLoggedIn={isLoggedIn} // Passiamo lo stato di login
                            isMyBook={false}
                            hasShipping={book.hasShipping}
                        />
                    ))}
                </div>
            </main>

            <Footer />
        </div>
    );
}