import { ThreatType } from "@/entities/problems/types";

export type ClientColorClasses = {
  bg: string;
  text: string;
  badge: string;
};

export const getClientColorClasses = (
  status: ThreatType | null
): ClientColorClasses => {
  switch (status) {
    case ThreatType.Critical:
      return {
        bg: "bg-red-100 hover:bg-red-200",
        text: "text-red-700",
        badge: "bg-red-300",
      };

    case ThreatType.Note:
      return {
        bg: "bg-yellow-100 hover:bg-yellow-200",
        text: "text-yellow-700",
        badge: "bg-yellow-300",
      };

    default:
      return {
        bg: "bg-gray-100 hover:bg-gray-200",
        text: "text-gray-500",
        badge: "",
      };
  }
};