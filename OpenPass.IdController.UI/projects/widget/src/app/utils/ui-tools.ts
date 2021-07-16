export const hexToRgb = (inputHex: string) => {
  const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
  const hex = inputHex.replace(shorthandRegex, (m, r, g, b) => r + r + g + g + b + b);
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);

  return result
    ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16),
      }
    : { r: 0, g: 0, b: 0 };
};

export const detectDarkColor = (inputColor: string) => {
  const { r, g, b } = hexToRgb(inputColor);
  const brightness = (r * 299 + g * 587 + b * 114) / 1000;
  return brightness < 125;
};
