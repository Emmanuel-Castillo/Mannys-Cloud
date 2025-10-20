type ActionSpanProps = {
  text: string;
  onClickButton: () => void;
};
const ActionSpan = ({ text, onClickButton }: ActionSpanProps) => {
  return (
    <span
      className="py-2 px-4 hover:bg-gray-600 cursor-pointer"
      onClick={onClickButton}
    >
      {text}
    </span>
  );
};

export default ActionSpan;
