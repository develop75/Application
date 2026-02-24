'use client';

import React, { useState } from 'react';
import Link from 'next/link';
import { Star, MapPin, ShoppingCart, BookOpen, Pencil, Trash2, Tag, Heart, Truck, Package } from 'lucide-react';

const BookCard = ({ id, title, author, rating, price, oldPrice, city, imageUrl, isLoggedIn, isMyBook = false, hasShipping = false }) => {
    const [imgError, setImgError] = useState(false);
    const [isFavorite, setIsFavorite] = useState(false);

    const hasImage = imageUrl && imageUrl.trim() !== "" && !imgError;
    const hasDiscount = !isMyBook && oldPrice && oldPrice > price;

    return (
        /* max-w a 350px | padding p-8 | Ombra più marcata */
        <div className="bg-white rounded-[3.5rem] p-8 shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-[0_20px_50px_rgba(0,0,0,0.1)] hover:-translate-y-2 transition-all duration-500 group flex flex-col h-full mx-auto w-full max-w-[350px] relative">

            {/* BADGE SCONTO XL */}
            {hasDiscount && (
                <div className="absolute top-10 left-10 z-10 bg-emerald-500 text-white text-[12px] font-black px-5 py-2.5 rounded-full shadow-lg flex items-center gap-2 uppercase tracking-widest">
                    <Tag size={14} />
                    OFFERTA
                </div>
            )}

            {/* TASTO PREFERITI XL */}
            {isLoggedIn && !isMyBook && (
                <button
                    onClick={(e) => { e.preventDefault(); setIsFavorite(!isFavorite); }}
                    className={`absolute ${hasDiscount ? 'top-10 right-10' : 'top-10 left-10'} z-20 p-4 rounded-[1.5rem] transition-all duration-300 shadow-md border border-slate-50
                        ${isFavorite ? 'bg-red-50 text-red-500 shadow-red-100' : 'bg-white/90 backdrop-blur-md text-slate-400 hover:text-red-500'}`}
                >
                    <Heart size={22} className={isFavorite ? 'fill-current' : ''} />
                </button>
            )}

            {/* AZIONI DI GESTIONE */}
            {isMyBook && (
                <div className="absolute top-10 right-10 z-10 flex gap-3">
                    <button className="bg-white/90 backdrop-blur-sm p-3 rounded-2xl text-slate-600 hover:text-blue-600 shadow-lg transition-all hover:scale-110 border border-slate-100"><Pencil size={20} /></button>
                    <button className="bg-white/90 backdrop-blur-sm p-3 rounded-2xl text-slate-600 hover:text-red-600 shadow-lg transition-all hover:scale-110 border border-slate-100"><Trash2 size={20} /></button>
                </div>
            )}

            {/* Immagine con angoli molto smussati */}
            <Link href={`/book/${id}`} className="relative aspect-[3/4] mb-8 overflow-hidden rounded-[2.5rem] bg-slate-50 flex items-center justify-center border border-slate-50 shadow-inner">
                {hasImage ? (
                    <img src={imageUrl} alt={title} onError={() => setImgError(true)} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-1000 ease-out" />
                ) : (
                    <div className="flex flex-col items-center justify-center gap-5 w-full h-full bg-slate-100 text-slate-400">
                        <BookOpen className="w-20 h-20 opacity-10" />
                        <span className="text-sm font-bold uppercase tracking-[0.2em] opacity-30">No Preview</span>
                    </div>
                )}
            </Link>

            <div className="flex flex-col flex-1 px-2">
                {/* Titolo più grande: text-xl */}
                <h3 className="text-xl font-extrabold text-slate-800 leading-tight mb-2 truncate">{title}</h3>
                <p className="text-slate-400 font-bold text-base mb-6">{author}</p>

                {/* Blocco Info Centrale */}
                <div className="flex flex-col gap-4 mb-8">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-1.5">
                            {[...Array(5)].map((_, i) => (
                                <Star key={i} size={16} className={`${i < rating ? 'fill-orange-400 text-orange-400' : 'text-slate-200'}`} />
                            ))}
                        </div>
                        <div className="flex items-center gap-2 text-slate-500 bg-slate-50 px-3 py-1 rounded-full">
                            <MapPin size={14} className="text-orange-400" />
                            <span className="text-[11px] font-black uppercase tracking-widest">{city}</span>
                        </div>
                    </div>

                    <div className={`flex items-center gap-2.5 px-4 py-2 rounded-2xl text-[11px] font-black uppercase tracking-widest w-fit shadow-sm
                        ${hasShipping ? 'bg-blue-50 text-blue-600' : 'bg-slate-50 text-slate-400'}`}>
                        {hasShipping ? (
                            <><Truck size={14} /> Spedizione Disponibile</>
                        ) : (
                            <><Package size={14} /> Scambio a mano</>
                        )}
                    </div>
                </div>

                {/* Prezzo e Carrello XL */}
                <div className="mt-auto pt-6 border-t border-slate-100 flex items-center justify-between">
                    <div className="flex flex-col">
                        {hasDiscount && <span className="text-base text-slate-400 line-through font-semibold italic">€{oldPrice.toFixed(2)}</span>}
                        <span className={`text-3xl font-black tracking-tighter ${hasDiscount ? 'text-emerald-600' : 'text-slate-900'}`}>€{price.toFixed(2)}</span>
                    </div>

                    {isLoggedIn && !isMyBook && (
                        <button className="bg-orange-500 hover:bg-orange-600 text-white p-5 rounded-[1.8rem] transition-all shadow-xl shadow-orange-100 active:scale-90">
                            <ShoppingCart size={26} />
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
};

export default BookCard;