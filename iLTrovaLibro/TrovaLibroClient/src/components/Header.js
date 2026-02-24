'use client';

import React from 'react';
import Link from 'next/link';
import { Search, Heart, ShoppingCart, User } from 'lucide-react';

const Header = ({ isLoggedIn = false }) => {
    return (
        // Usiamo h-20 fisso per l'header per poter calcolare bene lo spazio sotto
        <header className="fixed top-0 left-0 right-0 h-20 bg-white/95 backdrop-blur-md border-b border-gray-100 z-[100] shadow-sm">
            <div className="max-w-7xl mx-auto px-6 md:px-10 h-full flex items-center justify-between gap-4">

                {/* LOGO */}
                <div className="flex shrink-0">
                    <Link href="/Home" className="flex items-center group py-2">
                        <img
                            src="/logo.png"
                            alt="iLTrovaLibro"                        
                            className="h-20 md:h-20 w-auto object-contain transition-transform duration-300 group-hover:scale-105"
                        />
                        <span className="text-2xl md:text-3xl font-black tracking-tighter">
                            <span className="text-[#f17829]">iLTrova</span>
                            <span className="text-slate-800">Libro</span>
                        </span>
                    </Link>
                </div>

                {/* SEARCH BAR - Aggiunto flex-grow per evitare sovrapposizioni */}
                <div className="flex-grow max-w-xl hidden md:block">
                    <div className="relative flex items-center bg-slate-50 border border-slate-200 rounded-full p-1 focus-within:ring-2 focus-within:ring-orange-400/20 transition-all">
                        <input
                            type="text"
                            placeholder="Cerca un libro..."
                            className="flex-1 bg-transparent pl-4 pr-2 py-1.5 outline-none text-sm text-slate-600"
                        />
                        <button className="bg-orange-400 h-8 w-8 rounded-full hover:bg-orange-500 transition-colors flex items-center justify-center">
                            <Search className="w-4 h-4 text-white" />
                        </button>
                    </div>
                </div>

                {/* ACTIONS */}
                <div className="flex items-center gap-3 md:gap-6 flex-shrink-0">
                    <div className="flex items-center gap-3 md:gap-5">
                        {isLoggedIn && (
                            <Link href="/preferiti" className="relative p-1 text-slate-600 hover:text-red-500 transition-colors">
                                <Heart className="w-6 h-6" />
                                <span className="absolute -top-1 -right-1 bg-red-500 text-white text-[9px] rounded-full h-4 w-4 flex items-center justify-center font-bold">3</span>
                            </Link>
                        )}
                        <Link href="/carrello" className="relative p-1 text-slate-600 hover:text-orange-500 transition-colors">
                            <ShoppingCart className="w-6 h-6" />
                            <span className="absolute -top-1 -right-1 bg-orange-500 text-white text-[9px] rounded-full h-4 w-4 flex items-center justify-center font-bold">0</span>
                        </Link>
                    </div>

                    <div className="h-6 w-[1px] bg-slate-200 hidden sm:block" />

                    {!isLoggedIn ? (
                        <div className="flex items-center gap-3 text-sm font-bold">
                            <Link href="/Login" className="text-orange-500 hover:underline underline-offset-4">Login</Link>
                            <Link href="/Register" className="text-slate-800 hover:text-orange-500 transition-colors">Register</Link>
                        </div>
                    ) : (
                        <Link href="/profilo" className="flex items-center gap-2 group">
                            <div className="w-9 h-9 bg-slate-50 rounded-full flex items-center justify-center border border-slate-100 group-hover:bg-orange-50 transition-all">
                                <User className="w-5 h-5 text-slate-600 group-hover:text-orange-500" />
                            </div>
                        </Link>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Header;