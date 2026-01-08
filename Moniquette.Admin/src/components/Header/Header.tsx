import logoImg from "@/assets/logo.png";
import { NavLink } from "react-router-dom";

// export const Header = () => {
//   return (
//     <header className="w-full h-12 flex items-center justify-between px-4 border-b-2 border-gray-300">
//       <div className="flex items-center gap-2">
//         <img src={logoImg} alt="logo" className="w-7 h-7" />
//         <span className="text-2xl font-bold text-black leading-7">Moniquette</span>
//       </div>

//       <div className="flex items-center gap-5">
//         <button className="text-xl font-bold text-gray-400 hover:text-gray-600">
//           настройки
//         </button>
//         <button className="text-xl font-bold text-gray-400 hover:text-gray-600">
//           выйти
//         </button>
//       </div>
//     </header>
//   );
// };

export const Header = () => {
  return (
    <header className="w-full h-12 flex items-center justify-between px-4 border-b-2 border-gray-300">
      <NavLink
        to="/clients"
        className="flex items-center gap-2 text-black hover:text-gray-600"
      >
        <img src={logoImg} alt="logo" className="w-7 h-7" />
        <span className="text-2xl font-bold text-black leading-7">Moniquette</span>
      </NavLink>

      <div className="flex items-center gap-5">
        <NavLink
          to="/settings"
          className={({ isActive }) =>
            `text-xl font-bold ${isActive ? "text-blue-600" : "text-gray-400"
            } hover:text-gray-600`
          }
        >
          настройки
        </NavLink>

        <button className="text-xl font-bold text-gray-400 hover:text-gray-600">
          выйти
        </button>
      </div>
    </header>
  );
};
