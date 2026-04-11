import { useState } from "react";

type Props = {
  title: string;
  children: React.ReactNode;
  defaultOpen?: boolean;
  forceOpen?: boolean;
};

export const DetailsSection = ({
  title,
  children,
  defaultOpen = false,
  forceOpen = false,
}: Props) => {
  const [isOpen, setIsOpen] = useState(defaultOpen || forceOpen);

  const actualOpen = forceOpen ? true : isOpen;

  return (
    <section className="rounded-lg border border-gray-200 bg-white">
      <button
        type="button"
        onClick={() => {
          if (!forceOpen) {
            setIsOpen((prev) => !prev);
          }
        }}
        className={`
          flex w-full items-center justify-between px-4 py-3 text-left
          ${forceOpen ? "cursor-default" : "cursor-pointer"}
        `}
      >
        <span className="text-sm font-semibold text-black">{title}</span>

        {!forceOpen && (
          <span className="text-lg leading-none text-gray-500">
            {actualOpen ? "−" : "+"}
          </span>
        )}
      </button>

      {actualOpen && (
        <div className="border-t border-gray-200 px-4 py-4">
          {children}
        </div>
      )}
    </section>
  );
};