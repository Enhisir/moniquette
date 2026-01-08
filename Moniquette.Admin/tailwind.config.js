const colors = require('tailwindcss/colors')

module.exports = {
  plugins: [require("tailwind-scrollbar-hide")],
  theme: {
    extend: {
      colors: {
        ...colors,
        yellow: colors.yellow,
        amber: colors.amber,
        emerald: colors.emerald,
      }
    }
  }
}
