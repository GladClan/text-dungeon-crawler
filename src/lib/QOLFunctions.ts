// export function toTitleCase(str: string): string {
//     const arr = str.split(' ').map(str => {
//         const char = str.charAt(0).toUpperCase();
//         const remainder = str.substring(1);
//         return char + remainder;
//     })
//     return arr.reduce((s, n) => s + ' ' + n);
// }

export function toTitleCase(str: string): string {
    return str
        .split(' ')
        .map( word =>
            word.length > 0
                ? word.charAt(0).toUpperCase() + word.slice(1)
                : ''
        )
        .join(' ')
}