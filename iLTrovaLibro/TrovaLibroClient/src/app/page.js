import Image from "next/image";
import styles from "./page.module.css";

import { redirect } from 'next/navigation';

export default function RootPage() {    

    return (
          <div>
              {
                  redirect('/Home')
              }
          </div>   
  );
}
