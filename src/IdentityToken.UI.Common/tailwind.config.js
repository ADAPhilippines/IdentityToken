const colors = require('tailwindcss/colors')

module.exports = {
    mode: "jit",
    purge: {
        enabled: true,
        content: [
            './**/*.html',
            './TailwindClasses.razor'
        ],
    },
    darkMode: 'class', // or 'media' or 'class'
    theme: {
        container: {
            center: true,
        },
        fontFamily: {
            'sans': ['Poppins', 'sans-serif']
        },
        colors: {
            ...colors,
            'idt-indigo': '#4C4DE7',
            'idt-purple': '#7A60CA',
            'idt-gray': '#808088',
            'idt-gray-light': '#CFD1DC',
            'idt-gray-lightest': '#CFD1DC'
        },
        textColor: {
            ...colors,
            'primary': '#1E2835',
            'secondary': '#606066',
            'idt-gray': '#808088',
            'idt-indigo': '#4C4DE7',
            'idt-purple': '#7A60CA'
        },
        extend: {},
    },
    variants: {
        extend: {},
    },
    plugins: [],
}
