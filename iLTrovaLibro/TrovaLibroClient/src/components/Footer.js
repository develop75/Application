'use client';

import React from 'react';
import Link from 'next/link';
import { Facebook, Instagram, Twitter, Mail, MapPin, Phone, BookOpen } from 'lucide-react';

const Footer = () => {
    return (
        <footer className="bg-white border-t border-slate-200 pt-16 pb-8">
            <div className="max-w-7xl mx-auto px-10">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-12 mb-16">

                    {/* 1. INFO BRAND */}
                    <div className="flex flex-col gap-6">
                        <Link href="/" className="flex items-center group py-2">
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
                        <p className="text-slate-500 text-sm leading-relaxed">
                            La tua libreria digitale di fiducia. Compra, vendi e scambia i tuoi libri preferiti con facilità e sicurezza.
                        </p>
                        <div className="flex items-center gap-4 text-slate-400">
                            <Link href="#" className="hover:text-orange-500 transition-colors"><Instagram size={20} /></Link>
                            <Link href="#" className="hover:text-orange-500 transition-colors"><Facebook size={20} /></Link>
                            <Link href="#" className="hover:text-orange-500 transition-colors"><Twitter size={20} /></Link>
                        </div>
                    </div>

                    {/* 2. LINK RAPIDI */}
                    <div>
                        <h4 className="text-slate-800 font-bold mb-6">Esplora</h4>
                        <ul className="flex flex-col gap-4 text-sm font-medium text-slate-500">
                            <li><Link href="/" className="hover:text-orange-500 transition-colors">Tutti i libri</Link></li>
                            <li><Link href="#" className="hover:text-orange-500 transition-colors">I più venduti</Link></li>
                            <li><Link href="#" className="hover:text-orange-500 transition-colors">Nuovi arrivi</Link></li>
                            <li><Link href="#" className="hover:text-orange-500 transition-colors">Offerte speciali</Link></li>
                        </ul>
                    </div>

                    {/* 3. ACCOUNT */}
                    <div>
                        <h4 className="text-slate-800 font-bold mb-6">Il mio Account</h4>
                        <ul className="flex flex-col gap-4 text-sm font-medium text-slate-500">
                            <li><Link href="/profilo" className="hover:text-orange-500 transition-colors">Profilo</Link></li>
                            <li><Link href="/preferiti" className="hover:text-orange-500 transition-colors">I miei preferiti</Link></li>
                            <li><Link href="/my-books" className="hover:text-orange-500 transition-colors">I miei libri</Link></li>
                            <li><Link href="/carrello" className="hover:text-orange-500 transition-colors">Carrello</Link></li>
                        </ul>
                    </div>

                    {/* 4. CONTATTI */}
                    <div>
                        <h4 className="text-slate-800 font-bold mb-6">Contatti</h4>
                        <ul className="flex flex-col gap-4 text-sm font-medium text-slate-500">
                            <li className="flex items-center gap-3">
                                <MapPin size={18} className="text-orange-400" />
                                <span>Milano, Italia</span>
                            </li>
                            <li className="flex items-center gap-3">
                                <Phone size={18} className="text-orange-400" />
                                <span>+39 02 1234567</span>
                            </li>
                            <li className="flex items-center gap-3">
                                <Mail size={18} className="text-orange-400" />
                                <span>info@iltrovalibro.it</span>
                            </li>
                        </ul>
                    </div>
                </div>

                {/* BOTTOM BAR */}
                <div className="pt-8 border-t border-slate-100 flex flex-col md:flex-row justify-between items-center gap-4">
                    <p className="text-slate-400 text-xs">
                        © 2026 iLTrovaLibro. Tutti i diritti riservati.
                    </p>
                    <div className="flex gap-6 text-xs font-bold text-slate-400">
                        <Link href="#" className="hover:text-slate-600">Privacy Policy</Link>
                        <Link href="#" className="hover:text-slate-600">Termini e Condizioni</Link>
                    </div>
                </div>
            </div>
        </footer>
    );
};

export default Footer;